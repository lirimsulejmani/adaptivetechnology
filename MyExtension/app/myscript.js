//var replaced = $("body").html().replace(/text/g, 'replace');
//$("body").html(replaced);
$(document).ready(function () {
    //$("body").css("background-color", "red");
    //////////////////////////
   var uri = 'http://localhost:2519/api/default';
   // var uri = 'http://192.168.0.177:8080//api/default';
   //var uri = 'http://46.99.154.9:8080//api/default';
   var urlText = window.location.href;//"www.w3schools.com";

  
   document.body.ontimeupdate = function () { clearTimeout();};

    var data = {
        url: urlText
    };
    if (urlText.startsWith("https://")) {
        uri = 'https://localhost:44300/api/default';
        //  uri = 'https://192.168.0.177:9090/api/default';
        //var uri = 'https://46.99.154.9:9090//api/default';
       
        $.getJSON(uri, data, function (result) {
            var newDoc = document.open("text/html", "replace");
            newDoc.write(result);
            newDoc.close();
          //  $("body").html(result);
        });
    }
    else {
        $.getJSON(uri, data, function (result) {
            var newDoc = document.open("text/html", "replace");
            newDoc.write(result);
            newDoc.close();//$("body").html(result);
        });
    }
});