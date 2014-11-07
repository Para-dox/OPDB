<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <link href="Content/Site.css" rel="stylesheet" />
    <link href="Content/buttonStyle.css" rel="stylesheet" />
    <link href="~/Content/foundation/foundation.css" rel="stylesheet" />
    <meta name="viewport" content="width=device-width" />
    <title>Alcance RUM - Error</title>
</head>
<body>
    <div id="body">
            <section class="large-12 columns panel callout content-wrapper" style="text-align:center; align-content:center; margin:10% 10% 0 12%;">
                <img src="/Images/warning.ico" /><br />
                <h1><b>Error</b></h1><br />
                <h3>Se ha detectado contenido potencialmente peligroso al procesar esta acción.</h3>
                <h3>Por favor regrese e intente de nuevo.</h3><br />
                <a href="Home/Index" class="btn">Regresar</a>
            </section>
    </div>
</body>
</html>
