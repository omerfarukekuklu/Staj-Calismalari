var noteid = -1;
var modalCommentBodyId = "#modal_comment_body";

//Modal ın show metodu çalıştığında body kısmını yorumlar ile doldur.
$(function () {

    $('#modal_comment').on('show.bs.modal', function (e) {

        //Tıklanan buton elementini yakaladık.
        var btn = $(e.relatedTarget);

        //Butonda saklamış olduğunuz not id bilgisini aldık.
        noteid = btn.data("note-id");

        $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
    })
});

function doComment(btn, e, commentid, spanid) {

    var button = $(btn);
    var mode = button.data("edit-mode");

    if (e === "edit_clicked") {

        if (!mode) {
            button.data("edit-mode", true);
            button.removeClass("btn-warning");
            button.addClass("btn-success");

            var btnSpan = button.find("span");
            btnSpan.removeClass("glyphicon-edit");
            btnSpan.addClass("glyphicon-ok");

            $(spanid).addClass("editable");
            $(spanid).attr("contenteditable", true);
            $(spanid).focus();
        }
        else {
            button.data("edit-mode", false);
            button.removeClass("btn-success");
            button.addClass("btn-warning");

            var btnSpan = button.find("span");
            btnSpan.removeClass("glyphicon-ok");
            btnSpan.addClass("glyphicon-edit");

            $(spanid).removeClass("editable");
            $(spanid).attr("contenteditable", false);

            var txt = $(spanid).text();

            $.ajax({
                method: "POST",
                url: "/Comment/Edit/" + commentid,
                data: { text: txt }
            }).done(function (data) { //Ajax işlemi başarılı ise
                //Yorumu güncelleme işlemi başarılı ise
                if (data.result) {
                    //Yorumları gösteren partial page tekrar yüklenir
                    $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
                }
                //Yorumu güncelleme işlemi başarısız ise
                else {
                    alert("Yorum güncellemedi.");
                }

            }).fail(function () {  //Ajax işlemi başarısız ise
                alert("Sunucu ile bağlantı kurulamadı.");
            });
        }
    }
    else if (e === "delete_clicked") {
        var dialog_res = confirm("Yorum silinsin mi?");

        if (!dialog_res) return false;

        $.ajax({
            method: "GET",
            url: "/Comment/Delete/" + commentid
        }).done(function (data) { //Ajax işlemi başarılı ise
            //Yorumu silme işlemi başarılı ise
            if (data.result) {
                //Yorumları gösteren partial page tekrar yüklenir
                $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
            }
            //Yorumu silme işlemi başarısız ise
            else {
                alert("Yorum silinemedi.");
            }

        }).fail(function () { //Ajax işlemi başarısız ise
            alert("Sunucu ile bağlantı kurulamadı.");
        })
    }
    else if (e === "new_clicked") {

        var txt = $("#new_comment_text").val();

        if (!txt) {
            alert("Lütfen geçerli formatta bir yorum giriniz.");
        }

        var maxLength = 300;

        if (txt.length > maxLength) {
            alert("Yorum en fazla " + maxLength + " karakter uzunlukta olmalıdır.");
        }

        $.ajax({
            method: "POST",
            url: "/Comment/Create",
            data: { text: txt, "noteid": noteid }
        }).done(function (data) { //Ajax işlemi başarılı ise
            //Yorumu ekleme işlemi başarılı ise
            if (data.result) {
                //Yorumları gösteren partial page tekrar yüklenir
                $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
            }
            //Yorumu ekleme işlemi başarısız ise
            else {
                alert("Yorum eklenemedi.");
            }

        }).fail(function () { //Ajax işlemi başarısız ise
            alert("Sunucu ile bağlantı kurulamadı.");
        })
    }
}