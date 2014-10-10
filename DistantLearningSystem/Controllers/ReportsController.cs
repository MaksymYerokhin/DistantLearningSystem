using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Web.DataAccess;
using System.Web.UI;
using System.Drawing;
using DistantLearningSystem.Models.DataModels;

namespace DistantLearningSystem.Controllers
{
    using DistantLearningSystem.Models.LogicModels.Managers;

    public class ReportsController : Controller
    {
        //
        // GET: /Home/
        // Install-Package iTextSharp-LGPL
        public ActionResult Index()
        {
            return View();
        }


        // просто отчеты
        public void Report1()
        {
            // шаблон
            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.AddHeader("content-disposition", "attachment;filename=Краткая информация про студентов.pdf");
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, HttpContext.Response.OutputStream);
            document.Open();
            var db = new Models.DataModels.CourseDataBaseEntities();
            // шаблон

            // сколько объектов добавлено, 0 элемент - общее количество
            List<int> count_added = new List<int>();
            // иницализация
            List<List<Student>> groups = new List<List<Student>>();
            for (int i = 0; i < 7; i++)
            {
                groups.Add(new List<Student>());
                count_added.Add(0);
            }

            // распределяем студентов по группам, потом отсюда отображать будем
            foreach (var i in db.Students)
            {
                if (i.StudentGroup.Name == "ПИ-12-1")
                    groups[1].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-2")
                    groups[2].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-3")
                    groups[3].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-4")
                    groups[4].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-5")
                    groups[5].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-6")
                    groups[6].Add(i);
            }
            // считаем сколько по группам добавлено объктов
            foreach (var i in db.Concepts)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.Definitions)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.StudentConnections)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.References)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.Classifications)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.Formulations)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            // сколько в сумме
            count_added[0] = count_added[1] + count_added[2] + count_added[3] + count_added[4] + count_added[5] + count_added[6];

            document.AddAuthor("Admin");
            document.AddTitle("Общая информация о студентах");
            document.AddCreationDate();

            // не трогать, для отображения русских букв!!!
            string sylfaenpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\sylfaen.ttf";
            BaseFont sylfaen = BaseFont.CreateFont(sylfaenpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font head = new iTextSharp.text.Font(sylfaen, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLUE);
            iTextSharp.text.Font normal = new iTextSharp.text.Font(sylfaen, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLACK);
            iTextSharp.text.Font underline = new iTextSharp.text.Font(sylfaen, 10f, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.Color.BLACK);
            // не трогать, для отображения русских букв!!!


            document.Add(new Paragraph("Список студентов, их общей информации, а также их оценки за добавленные объекты различных типов. Дата последнего посещения и оценки за активность позволяют быстро оценить общую и текущую активность и успешность студента, а также сравнить таковую с другими студентами", head));
            document.Add(new Paragraph(" ", normal));
            document.Add(new Paragraph(" ", normal));

            // задаем кол-во столбцов
            PdfPTable table = new PdfPTable(5);
            // заполняем шапку
            table.AddCell(new Phrase("Группа", normal));
            table.AddCell(new Phrase("ФИО студента", normal));
            table.AddCell(new Phrase("Последнее посещение", normal));
            table.AddCell(new Phrase("Тип объекта", normal));
            table.AddCell(new Phrase("Оценка", normal));


            groups[1].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[2].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[3].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[4].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[5].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[6].Sort((x, y) => x.LastName.CompareTo(y.LastName));

            // заполняем таблицу
            for (int k = 1; k < 7; k++)
            {
                for (int i = 0; i < groups[k].Count(); i++)
                {

                    foreach (var j in db.Formulations)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                        {
                            if (j.Status == 0)
                                continue;
                            table.AddCell(new Phrase(groups[k][i].StudentGroup.Name, normal));
                            table.AddCell(new Phrase((groups[k][i].LastName + " " + groups[k][i].Name), normal));
                            table.AddCell(new Phrase(groups[k][i].LastVisitDate.ToString(), normal));
                            table.AddCell(new Phrase("Формулировка", normal));
                            table.AddCell(j.Status.ToString());
                        }
                    }
                    foreach (var j in db.Definitions)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                        {
                            if (j.Status == 0)
                                continue;
                            table.AddCell(new Phrase(groups[k][i].StudentGroup.Name, normal));
                            table.AddCell(new Phrase((groups[k][i].LastName + " " + groups[k][i].Name), normal));
                            table.AddCell(new Phrase(groups[k][i].LastVisitDate.ToString(), normal));
                            table.AddCell(new Phrase("Определение", normal));
                            table.AddCell(j.Status.ToString());
                        }
                    }
                    foreach (var j in db.Concepts)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                        {
                            if (j.Status == 0)
                                continue;
                            table.AddCell(new Phrase(groups[k][i].StudentGroup.Name, normal));
                            table.AddCell(new Phrase((groups[k][i].LastName + " " + groups[k][i].Name), normal));
                            table.AddCell(new Phrase(groups[k][i].LastVisitDate.ToString(), normal));
                            table.AddCell(new Phrase("Понятие", normal));
                            table.AddCell(j.Status.ToString());
                        }
                    }
                    foreach (var j in db.Classifications)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                        {
                            if (j.Status == 0)
                                continue;
                            table.AddCell(new Phrase(groups[k][i].StudentGroup.Name, normal));
                            table.AddCell(new Phrase((groups[k][i].LastName + " " + groups[k][i].Name), normal));
                            table.AddCell(new Phrase(groups[k][i].LastVisitDate.ToString(), normal));
                            table.AddCell(new Phrase("Классификация", normal));
                            table.AddCell(j.Status.ToString());
                        }
                    }
                    foreach (var j in db.StudentConnections)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                        {
                            if (j.Status == 0)
                                continue;
                            table.AddCell(new Phrase(groups[k][i].StudentGroup.Name, normal));
                            table.AddCell(new Phrase((groups[k][i].LastName + " " + groups[k][i].Name), normal));
                            table.AddCell(new Phrase(groups[k][i].LastVisitDate.ToString(), normal));
                            table.AddCell(new Phrase("Связь", normal));
                            table.AddCell(j.Status.ToString());
                        }
                    }
                }
            }

            document.Add(table);

            document.Add(new Paragraph("Отчет сформирован " + DateTime.Now.ToString(), head));
            document.Close();
            HttpContext.Response.Write(document);
            HttpContext.Response.End();
        }

        public void Report2()
        {
            // шаблон
            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.AddHeader("content-disposition", "attachment;filename=Краткая информация про студентов.pdf");
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, HttpContext.Response.OutputStream);
            document.Open();
            var db = new Models.DataModels.CourseDataBaseEntities();
            // шаблон

            // сколько объектов добавлено, 0 элемент - общее количество
            List<int> count_added = new List<int>();
            // иницализация
            List<List<Student>> groups = new List<List<Student>>();
            for (int i = 0; i < 7; i++)
            {
                groups.Add(new List<Student>());
                count_added.Add(0);
            }

            // распределяем студентов по группам, потом отсюда отображать будем
            foreach (var i in db.Students)
            {
                if (i.StudentGroup.Name == "ПИ-12-1")
                    groups[1].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-2")
                    groups[2].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-3")
                    groups[3].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-4")
                    groups[4].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-5")
                    groups[5].Add(i);
                if (i.StudentGroup.Name == "ПИ-12-6")
                    groups[6].Add(i);
            }
            // считаем сколько по группам добавлено объктов
            foreach (var i in db.Concepts)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.Definitions)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.StudentConnections)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.References)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.Classifications)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            foreach (var i in db.Formulations)
            {
                count_added[i.Student.StudentGroup.Name[i.Student.StudentGroup.Name.Length - 1] - '0']++;
            }
            // сколько в сумме
            count_added[0] = count_added[1] + count_added[2] + count_added[3] + count_added[4] + count_added[5] + count_added[6];

            document.AddAuthor("Admin");
            document.AddTitle("Общая информация о студентах");
            document.AddCreationDate();

            // не трогать, для отображения русских букв!!!
            string sylfaenpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\sylfaen.ttf";
            BaseFont sylfaen = BaseFont.CreateFont(sylfaenpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font head = new iTextSharp.text.Font(sylfaen, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLUE);
            iTextSharp.text.Font normal = new iTextSharp.text.Font(sylfaen, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLACK);
            iTextSharp.text.Font underline = new iTextSharp.text.Font(sylfaen, 10f, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.Color.BLACK);
            // не трогать, для отображения русских букв!!!


            document.Add(new Paragraph("Список студентов, их общей информации. Дата регистрации, дата последнего посещенияи количество добавленных сущностей позволяет быстро оценить общую и текущую активность студента, а также сравнить таковую по группам", head));
            document.Add(new Paragraph(" ", normal));
            document.Add(new Paragraph(" ", normal));

            // задаем кол-во столбцов
            PdfPTable table = new PdfPTable(6);
            // заполняем шапку
            table.AddCell(new Phrase("Группа", normal));
            table.AddCell(new Phrase("ФИО студента", normal));
            table.AddCell(new Phrase("Дата регистрации", normal));
            table.AddCell(new Phrase("Последнее посещение", normal));
            table.AddCell(new Phrase("Добавлено объектов", normal));
            table.AddCell(new Phrase("Итоговая оценка", normal));
            groups[1].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[2].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[3].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[4].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[5].Sort((x, y) => x.LastName.CompareTo(y.LastName));
            groups[6].Sort((x, y) => x.LastName.CompareTo(y.LastName));

            // заполняем таблицу
            for (int k = 1; k < 7; k++)
            {
                for (int i = 0; i < groups[k].Count(); i++)
                {
                    table.AddCell(new Phrase(groups[k][i].StudentGroup.Name, normal));
                    table.AddCell(new Phrase(groups[k][i].LastName + " " + groups[k][i].Name, normal));
                    table.AddCell(new Phrase(groups[k][i].RegDate.ToString(), normal));
                    table.AddCell(new Phrase(groups[k][i].LastVisitDate.ToString(), normal));
                    int studcount = 0;
                    foreach (var j in db.Formulations)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                            studcount++;
                    }
                    foreach (var j in db.Definitions)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                            studcount++;
                    }
                    foreach (var j in db.Concepts)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                            studcount++;
                    }
                    foreach (var j in db.Classifications)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                            studcount++;
                    }
                    foreach (var j in db.StudentConnections)
                    {
                        if (j.Student.Id == groups[k][i].Id)
                            studcount++;
                    }
                    table.AddCell(studcount.ToString());
                    table.AddCell(DataManager.Student.CalculateUserMark(groups[k][i], 100).ToString());
                }
                PdfPCell cell = new PdfPCell(new Phrase(("Количество добавленных объектов в " + k.ToString() + " группе : " + count_added[k].ToString()), normal));
                cell.Colspan = 6;
                cell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                table.AddCell(cell);
            }

            PdfPCell cell2 = new PdfPCell(new Phrase(("Общее количество добавленных объектов : " + count_added[0].ToString()), normal));
            cell2.Colspan = 6;
            cell2.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell2);

            document.Add(table);

            document.Add(new Paragraph("Отчет сформирован " + DateTime.Now.ToString(), head));
            document.Close();
            HttpContext.Response.Write(document);
            HttpContext.Response.End();
        }
    }
}
