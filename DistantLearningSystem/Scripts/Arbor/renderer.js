function RecursiveRemove(obj) {
    var edges = sys.getEdgesFrom(obj);

    for (var i = 0; i < edges.length; i++) {
        var tot = edges[i].target;
        if ((sys.getEdgesFrom(tot)).length == 0) {
            sys.pruneNode(tot);
        }
        else {
            RecursiveRemove(tot);
            sys.pruneNode(tot);
        }
    }
    return;
}

function run_ajax(idclass, id1, Concept, reallyselected) {
    $.ajax({
        url: "/Arbor/CheckConnection", data: { idclass: idclass, id1: id1, id2: Concept.data.concid }, success: function (data1) {
            var isvalid = true;
            if (data1.Length == 0)
                isvalid = false;
            var new_edge = sys.addEdge(reallyselected.node, Concept, { 'directed': true, 'color': 'blue', 'confirm': isvalid });
        }
    });
}

(function () {

    Renderer = function (canvas) {
        var canvas = $(canvas).get(0)
        var ctx = canvas.getContext("2d");
        var gfx = arbor.Graphics(canvas)
        var particleSystem = null

        var that = {
            init: function (system) {
                particleSystem = system
                particleSystem.screenSize(canvas.width, canvas.height)
                particleSystem.screenPadding(200)

                that.initMouseHandling()
            },

            redraw: function () {
                if (!particleSystem) return
                gfx.clear() // convenience ?: clears the whole canvas rect  
               // var friction1 = countNodes() === 1 ? 1.0 : 0.5;
               // sys.parameters({ friction: friction1 });
                // draw the nodes & save their bounds for edge drawing
                var nodeBoxes = {}
                particleSystem.eachNode(function (node, pt) {
                    // node: {mass:#, p:{x,y}, name:"", data:{}}
                    // pt:   {x:#, y:#}  node position in screen coords

                    // determine the box size and round off the coords if we'll be 
                    // drawing a text label (awful alignment jitter otherwise...)
                    var label = node.data.label || ""
                    var w = ctx.measureText("" + label).width + 10
                    if (!("" + label).match(/^[ \t]*$/)) {
                        pt.x = Math.floor(pt.x)
                        pt.y = Math.floor(pt.y)
                    } else {
                        label = null
                    }

                    // draw a rectangle centered at pt
                    if (node.data.color) ctx.fillStyle = node.data.color
                    else ctx.fillStyle = "rgba(0,0,0,.2)"
                    if (node.data.color == 'none') ctx.fillStyle = "white"

                    if (node.data.shape == 'dot') {
                        gfx.oval(pt.x - w / 2, pt.y - w / 2, w, w, { fill: ctx.fillStyle })
                        nodeBoxes[node.name] = [pt.x - w / 2, pt.y - w / 2, w, w]
                    } else {
                        gfx.rect(pt.x - w / 2, pt.y - 10, w, 20, 4, { fill: ctx.fillStyle })
                        nodeBoxes[node.name] = [pt.x - w / 2, pt.y - 11, w, 22]
                    }

                    // draw the text
                    if (label) {
                        ctx.font = "12px Helvetica"
                        ctx.textAlign = "center"
                        ctx.fillStyle = "white"
                        if (node.data.color == 'none') ctx.fillStyle = '#333333'
                        ctx.fillText(label || "", pt.x, pt.y + 4)
                        ctx.fillText(label || "", pt.x, pt.y + 4)
                    }

                })


                // draw the edges
                particleSystem.eachEdge(function (edge, pt1, pt2) {
                    // edge: {source:Node, target:Node, length:#, data:{}}
                    // pt1:  {x:#, y:#}  source position in screen coords
                    // pt2:  {x:#, y:#}  target position in screen coords

                    var weight = edge.data.weight
                    var color = edge.data.color
                    if (!color || ("" + color).match(/^[ \t]*$/)) color = null

                    if (edge.data.confirm == false)
                        color = 'red';
                    // find the start point
                    var tail = intersect_line_box(pt1, pt2, nodeBoxes[edge.source.name])
                    var head = intersect_line_box(tail, pt2, nodeBoxes[edge.target.name])

                    ctx.save()
                    ctx.beginPath()
                    ctx.lineWidth = (!isNaN(weight)) ? parseFloat(weight) : 2
                    if (edge.data.confirm == false)
                        ctx.lineWidth = 1;
                    ctx.strokeStyle = (color) ? color : color
                    ctx.fillStyle = null

                    ctx.moveTo(tail.x, tail.y)
                    ctx.lineTo(head.x, head.y)
                    ctx.stroke()
                    ctx.restore()

                    // draw an arrowhead if this is a -> style edge
                    if (edge.data.directed) {
                        ctx.save()
                        // move to the head position of the edge we just drew
                        var wt = !isNaN(weight) ? parseFloat(weight) : 1
                        var arrowLength = 6 + wt
                        var arrowWidth = 2 + wt
                        ctx.fillStyle = (color) ? color : color
                        ctx.translate(head.x, head.y);
                        ctx.rotate(Math.atan2(head.y - tail.y, head.x - tail.x));

                        // delete some of the edge that's already there (so the point isn't hidden)
                        ctx.clearRect(-arrowLength / 2, -wt / 2, arrowLength / 2, wt)

                        // draw the chevron
                        ctx.beginPath();
                        ctx.moveTo(-arrowLength, arrowWidth);
                        ctx.lineTo(0, 0);
                        ctx.lineTo(-arrowLength, -arrowWidth);
                        ctx.lineTo(-arrowLength * 0.8, -0);
                        ctx.closePath();
                        ctx.fill();
                        ctx.restore()
                    }
                }) 
            },
            initMouseHandling: function () {
                // no-nonsense drag and drop (thanks springy.js)
                selected = null;
                nearest = null;
                var dragged = null;
                // set up a handler object that will initially listen for mousedowns then
                // for moves and mouseups while dragging
                var handler = {
                    clicked: function (e) {
                        var pos = $(canvas).offset();
                        _mouseP = arbor.Point(e.pageX - pos.left, e.pageY - pos.top);
                        selected = nearest = dragged = particleSystem.nearest(_mouseP);
                        dragged.node.fixed = true;

                        $(canvas).bind('mousemove', handler.dragged);
                        $(window).bind('mouseup', handler.dropped);
                        return false;
                    },

                    dragged: function (e) {
                        var pos = $(canvas).offset();
                        var s = arbor.Point(e.pageX - pos.left, e.pageY - pos.top);
                        var p = particleSystem.fromScreen(s);
                        dragged.node.p = p;
                        return false;
                    },

                    dropped: function (e) {
                        dragged.node.fixed = false;
                        dragged = null;
                        selected = null;
                        $(canvas).unbind('mousemove', handler.dragged);
                        $(window).unbind('mouseup', handler.dropped);
                        _mouseP = null;
                        return false;
                    },

                    reallyclicked: function (e) {
                        var position = $(canvas).offset();
                        var mouseP = arbor.Point(e.pageX - position.left, e.pageY - position.top);
                        var reallyselected = particleSystem.nearest(mouseP);

                        if (reallyselected.node.name == "Classifications") {
                            return false;
                        }

                        if (reallyselected.node.name == "AddClassification") {
                            if (document.getElementById('follow').checked == true) {
                                window.location.href = "/Dashboard/AddClassification";
                                return false;
                            }
                            return false;
                        }

                        var arr = sys.getEdgesTo(reallyselected.node.name);
                        // если выбрана классификаци€
                        if (arr.length == 1 && arr[0].source.name == "Classifications") {
                            flag = true;

                            if (document.getElementById('linke').checked == true) {

                                if (node11 == null) {
                                    node1col = reallyselected.node.data.color;
                                    reallyselected.node.data.color = "green";
                                    node11 = reallyselected.node;
                                    selected_classification = reallyselected.node.data.classid;
                                    document.getElementById('linke').disabled = true;
                                    return;
                                }
                                return;
                            }
                            if ((sys.getEdgesFrom(reallyselected.node.name)).length == 0) {
                                // очищаем другие классификации
                                var allclass = sys.getEdgesFrom(Classifications.name);
                                for (var i = 0; i < allclass.length; i++) {
                                    if (allclass[i].target.name != reallyselected.node.name) {
                                        RecursiveRemove(allclass[i].target);
                                    }
                                }

                                var idclass = reallyselected.node.data.classid;
                                selected_classification = idclass;
                                var concs = [];
                                var c = 0;
                                $.ajax({
                                    url: "/Arbor/Concepts", data: { data: idclass }, success: function (data) {
                                        for (var i = 0; i < data.Length; i++) {
                                            var Concept = sys.addNode(data.Data[i], { 'color': 'purple', 'shape': 'rectangle', 'label': data.Data[i], 'concid': data.Value[i] });
                                            run_ajax(idclass, -1, Concept, reallyselected);
                                        }
                                    }
                                });
                            }
                            else {
                                RecursiveRemove(reallyselected.node);
                            }
                        }
                        else {
                            // если выбрано пон€тие
                            if (document.getElementById('linke').checked == true) {
                                // выбираем родител€
                                if (node11 == null) {
                                    node1col = reallyselected.node.data.color;
                                    reallyselected.node.data.color = "green";
                                    node11 = reallyselected.node;
                                    document.getElementById('linke').disabled = true;
                                    return;
                                }
                                // выбираем ребенка
                                node12 = reallyselected.node;
                                if (node11 == node12) {
                                    node12 = null;
                                    alert("ѕон€ти€ должны быть разные!");
                                    return;
                                }
                                node2col = reallyselected.node.data.color;
                                reallyselected.node.data.color = "green";

                                var idclass = selected_classification;
                               
                                var id1;
                                arr = sys.getEdgesTo(node11.name);
                                // если родитель -  классификаци€
                                if (arr.length == 1 && arr[0].source.name == "Classifications")
                                    id1 = -1;
                                else id1 = node11.data.concid;
                                var id2 = reallyselected.node.data.concid;

                                var nn1 = node11;
                                var nn2 = node12;
                                if (state == "Student") {
                                    $.ajax({
                                        url: "/Arbor/AddConnection", data: { idclass: idclass, id1: id1, id2: id2 }, success: function (data) {
                                            var new_edge = sys.addEdge(nn1, nn2, { 'directed': true });
                                        }
                                    });

                                    node11.data.color = node1col;
                                    node12.data.color = node2col;
                                    node11 = null;
                                    node12 = null;

                                    document.getElementById('linke').checked = false;
                                    document.getElementById('linke').disabled = false;
                                }
                                if (state == "Lecturer") {
                                    if (document.getElementById('confirmation').checked == true) {
                                        var act1 = 1;
                                        $.ajax({
                                            url: "/Arbor/DeleteConfirmConnection", data: { idclass: idclass, id1: id1, id2: id2, act: act1 }, success: function (data) {
                                                var edge = sys.getEdges(nn1, nn2);
                                                edge[0].data.confirm = true;
                                            }
                                        });
                                    }
                                    else {
                                        var act0 = 0;
                                        $.ajax({
                                            url: "/Arbor/DeleteConfirmConnection", data: { idclass: idclass, id1: id1, id2: id2, act: act0 }, success: function (data) {
                                                var edge = sys.getEdges(nn1, nn2);
                                                sys.pruneEdge(edge[0]);
                                            }
                                        });
                                    }
                                    document.getElementById("confirm_text").innerHTML = "Ќедоступно";
                                    document.getElementById("confirmation").checked = false;
                                    document.getElementById("confirmation").disabled = true;
                                    document.getElementById('linke').checked = false;
                                    document.getElementById('linke').disabled = false;

                                    node11.data.color = node1col;
                                    node12.data.color = node2col;
                                    node11 = null;
                                    node12 = null;
                                }                               

                                return;
                            }

                            if (document.getElementById('follow').checked == true) {

                                var idconc = reallyselected.node.data.concid;
                                var url = "/Search/Concept/" + idconc;
                                window.location.href = url;
                               
                                return;
                            }
                            if ((sys.getEdgesFrom(reallyselected.node.name)).length == 0) {
                                //загрузить пон€ти€-дети от пон€ти€
                                var idclass = selected_classification;
                                var idconc = reallyselected.node.data.concid;
                                $.ajax({
                                    url: "/Arbor/ByConcepts", data: { idclass: idclass, idconc: idconc }, success: function (data) {
                                        for (var i = 0; i < data.Length; i++) {
                                            var Concept = sys.addNode(data.Data[i], { 'color': 'purple', 'shape': 'rectangle', 'label': data.Data[i], 'concid': data.Value[i] });
                                            run_ajax(idclass, idconc, Concept, reallyselected);
                                        }
                                    }
                                });
                            }
                            else {
                                RecursiveRemove(reallyselected.node);
                            }
                        }

                        return;
                    }
                }
                $(canvas).mousedown(handler.clicked);
                $(canvas).click(handler.reallyclicked);
            }

        }

        // helpers for figuring out where to draw arrows (thanks springy.js)
        var intersect_line_line = function (p1, p2, p3, p4) {
            var denom = ((p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y));
            if (denom === 0) return false // lines are parallel
            var ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denom;
            var ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denom;

            if (ua < 0 || ua > 1 || ub < 0 || ub > 1) return false
            return arbor.Point(p1.x + ua * (p2.x - p1.x), p1.y + ua * (p2.y - p1.y));
        }

        var intersect_line_box = function (p1, p2, boxTuple) {
            var p3 = { x: boxTuple[0], y: boxTuple[1] },
                w = boxTuple[2],
                h = boxTuple[3]

            var tl = { x: p3.x, y: p3.y };
            var tr = { x: p3.x + w, y: p3.y };
            var bl = { x: p3.x, y: p3.y + h };
            var br = { x: p3.x + w, y: p3.y + h };

            return intersect_line_line(p1, p2, tl, tr) ||
                  intersect_line_line(p1, p2, tr, br) ||
                  intersect_line_line(p1, p2, br, bl) ||
                  intersect_line_line(p1, p2, bl, tl) ||
                  false
        }

        return that
    }

})()