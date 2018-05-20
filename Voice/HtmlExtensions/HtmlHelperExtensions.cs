using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.Mvc;
using Voice.Models;

namespace Voice.HtmlExtensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString CollapseElemets(Patient patient, Visit visit, List<WavFile> files, int i/*, WavFile wavFile*/)
        {
            List<string> info = new List<string>();
            var paddingDiv = new TagBuilder("div");
            if(i%2==0)
                paddingDiv.MergeAttribute("style", "padding-right: 500px");
            else
                paddingDiv.MergeAttribute("style", "padding-left: 500px");
            foreach(var file in files)
            {
                info.Add($"Имя файла: <a class=\"playElement\" id=\"{file.Name}\">{file.Name}</a> <a href=\"#\"> " +
                    $"<span class=\"glyphicon glyphicon-remove\" id=\"deleteAudio\" name=\"{file.Name}_{visit.Id}\"" +
                    $"style=\"color: red\"></span> </a> <br> " +
                    $"Джиттер: {file.Jitter}% <br>" +
                    $"Шиммер: {file.Shimmer}% <br>" +
                    $"Отношение гармоника/шум: {file.HNR} дБ <br>" +
                    $"Интенсивность: {Math.Round(file.Intensity, 3)} дБ <br>");
            }
            var panelDiv = new TagBuilder("div");
            panelDiv.AddCssClass("panel panel-default col-lg-8");
            //panelDiv.MergeAttribute("style", "padding-left: 10px");
            var panelHeading = new TagBuilder("div");
            panelHeading.AddCssClass("panel-heading");
            var panelTitle = new TagBuilder("h4");
            panelTitle.AddCssClass("panel-title");
            var link = new TagBuilder("a");
            link.MergeAttribute("data-toggle", "collapse");
            link.MergeAttribute("data-parent", "#accordion");
            link.MergeAttribute("href", "#collapse" + i);
            link.InnerHtml =  "Посещение от " + visit.DateTime.Day + "." + visit.DateTime.Month + "." + visit.DateTime.Year + " в " 
                + visit.DateTime.TimeOfDay.Hours + ":" + visit.DateTime.TimeOfDay.Minutes + ":" + visit.DateTime.TimeOfDay.Seconds;
            var container = new TagBuilder("div");
            container.GenerateId("collapse" + i);
            container.AddCssClass("panel-collapse collapse");
            var panelBody = new TagBuilder("div");
            panelBody.AddCssClass("panel-body");
            panelBody.GenerateId(visit.Id.ToString());
            foreach(var str in info)
                panelBody.InnerHtml += str + "<br>";
            TagBuilder br = new TagBuilder("br");
            

            container.InnerHtml = panelBody.ToString(TagRenderMode.Normal);
            panelTitle.InnerHtml = link.ToString(TagRenderMode.Normal);
            panelHeading.InnerHtml = panelTitle.ToString(TagRenderMode.Normal);
            panelDiv.InnerHtml = panelHeading.ToString(TagRenderMode.Normal) + container.ToString(TagRenderMode.Normal);
            paddingDiv.InnerHtml = panelDiv.ToString(TagRenderMode.Normal);
            string html;
            if (i % 2 == 1)
            {
                html = paddingDiv.ToString(TagRenderMode.Normal) + br.ToString(TagRenderMode.Normal) + br.ToString(TagRenderMode.Normal);
            }
            else
               html = paddingDiv.ToString(TagRenderMode.Normal);
            return MvcHtmlString.Create(html);
        }
    }
}