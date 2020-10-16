$(function () {

    $('#modal_note_detail').on('show.bs.modal', function (e) {

        //Tıklanan buton elementini yakaladık.
        var btn = $(e.relatedTarget);

        //Butonda saklamış olduğunuz not id bilgisini aldık.
        noteid = btn.data("note-id");

        $("#modal_note_detail_body").load("/Note/GetNoteText/" + noteid);
    })
});