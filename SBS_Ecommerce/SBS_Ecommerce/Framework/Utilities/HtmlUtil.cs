using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;

namespace SBS_Ecommerce.Framework.Utilities
{
    public class HtmlUtil
    {

        public static MvcHtmlString InitJS(string domain = "")
        {
            StringBuilder builder = new StringBuilder();
            using (StringWriter writer = new StringWriter(builder))
            {
                using (HtmlTextWriter html = new HtmlTextWriter(writer))
                {
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery/jquery.min.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datetimepicker/js/bootstrap-datetimepicker.js");

                    GenScript(html, domain + "/amaretti/light/assets/lib/bootstrap/dist/js/bootstrap.min.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.nanoscroller/javascripts/jquery.nanoscroller.min.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.gritter/js/jquery.gritter.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/select2/js/select2.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datepicker/js/bootstrap-datepicker.js");

                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.niftymodals/js/jquery.modalEffects.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.sparkline/jquery.sparkline.min.js");

                    GenScript(html, domain + "/amaretti/light/assets/lib/prettify/prettify.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery-flot/jquery.flot.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery-flot/jquery.flot.pie.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery-flot/jquery.flot.resize.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery-flot/plugins/jquery.flot.orderBars.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery-flot/plugins/curvedLines.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/jquery-jvectormap-1.2.2.min.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/maps/jquery-jvectormap-us-merc-en.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/maps/jquery-jvectormap-world-mill-en.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/maps/jquery-jvectormap-uk-mill-en.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/maps/jquery-jvectormap-fr-merc-en.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/maps/jquery-jvectormap-us-il-chicago-mill-en.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/maps/jquery-jvectormap-au-mill-en.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/maps/jquery-jvectormap-in-mill-en.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/maps/jquery-jvectormap-map.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery.vectormap/maps/jquery-jvectormap-ca-lcc-en.js");

                    GenScript(html, domain + "/amaretti/light/assets/lib/jquery-ui/jquery-ui.min.js");
                    GenScript(html, domain + "/amaretti/light/assets/js/main.js");

                    GenScript(html, domain + "/amaretti/custom/lib/chartjs/chart.min.js");
                    GenScript(html, domain + "/amaretti/custom/lib/countup/countUp.min.js");

                    GenScript(html, domain + "/Content/Admin/js/commonAdmin.js");
                    GenScript(html, domain + "/Content/Admin/js/app-page-profile.js");
                    GenScript(html, domain + "/Content/Admin/js/app-validation.js");
                    GenScript(html, domain + "/Content/Admin/js/app-site.js");

                    GenScript(html, domain + "/Scripts/jquery.validate.js");
                    GenScript(html, domain + "/Scripts/jquery.validate.unobtrusive.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datatables/js/jquery.dataTables.min.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datatables/js/dataTables.bootstrap.min.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datatables/js/dataTables.jqueryui.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datatables/js/dataTables.foundation.js");

                    //GenScript(html, domain + "/amaretti/light/assets/lib/datatables/plugins/buttons/js/buttons.bootstrap.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datatables/plugins/buttons/js/buttons.colVis.js");
                    //GenScript(html, domain + "/amaretti/light/assets/lib/datatables/plugins/buttons/js/buttons.flash.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datatables/plugins/buttons/js/buttons.html5.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datatables/plugins/buttons/js/buttons.print.js");
                    GenScript(html, domain + "/amaretti/light/assets/lib/datatables/plugins/buttons/js/dataTables.buttons.js");



                    //GenScript(html, domain + "/Scripts/app-validate.js");
                    //GenScript(html, domain + "/Scripts/app-control.js");
                }
            }
            return MvcHtmlString.Create(builder.ToString());
        }

        public static void GenScript(HtmlTextWriter html, string jsfile)
        {
            html.AddAttribute(HtmlTextWriterAttribute.Src, jsfile);
            html.RenderBeginTag(HtmlTextWriterTag.Script);
            html.RenderEndTag();
        }

        public static MvcHtmlString InitCSS(string domain = "")
        {
            StringBuilder builder = new StringBuilder();
            using (StringWriter writer = new StringWriter(builder))
            {
                using (HtmlTextWriter html = new HtmlTextWriter(writer))
                {
                    GenCss(html, domain + "/amaretti/light/assets/lib/stroke-7/style.css");
                    GenCss(html, domain + "/amaretti/light/assets/lib/jquery.nanoscroller/css/nanoscroller.css");
                    GenCss(html, domain + "/amaretti/light/assets/lib/datetimepicker/css/bootstrap-datetimepicker.css");
                    GenCss(html, domain + "/amaretti/light/assets/lib/datepicker/css/datepicker.css");
                    GenCss(html, domain + "/amaretti/light/assets/lib/datatables/css/dataTables.bootstrap.css");
                    GenCss(html, domain + "/amaretti/light/assets/lib/select2/css/select2.css");

                    GenCss(html, domain + "/amaretti/offlinejs/css/offline_style.css");

                    GenCss(html, domain + "/amaretti/light/assets/css/style.css");
                    GenCss(html, domain + "/amaretti/custom/css/customstyle.css");
                    GenCss(html, domain + "/amaretti/custom/css/custom.css");

                    GenCss(html, domain + "/amaretti/custom/css/blockmanager.css");
                    GenCss(html, domain + "/amaretti/custom/css/layoutmanager.css");
                    GenCss(html, domain + "/amaretti/custom/css/maketingmanager.css");
                    GenCss(html, domain + "/amaretti/custom/css/menumanager.css");
                    GenCss(html, domain + "/amaretti/custom/css/pagemanager.css");
                    GenCss(html, domain + "/amaretti/custom/css/sendmailmanager.css");
                }

            }
            return MvcHtmlString.Create(builder.ToString());
        }

        public static void GenCss(HtmlTextWriter html, string cssfile, bool media = false)
        {
            if (media)
                html.AddAttribute("media", "all");
            html.AddAttribute(HtmlTextWriterAttribute.Href, cssfile);
            html.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
            html.RenderBeginTag(HtmlTextWriterTag.Link);
            html.RenderEndTag();
        }
    }
}