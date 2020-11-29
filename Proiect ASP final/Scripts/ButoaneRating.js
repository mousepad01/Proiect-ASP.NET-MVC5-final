// bucata de cod care se ocupa cu adaugarea unui comentariu

var ratingNouBtn = document.getElementById("ratingNouBtn");
var ratingNouDiv = document.getElementById("reviewNou");

if (ratingNouBtn != null) {

    var idProdus = ratingNouBtn.value;

    $(ratingNouBtn).on("click", function () {

        $(ratingNouBtn).remove();

        $(ratingNouDiv).load("/ProdusRating/AdaugaRating/" + idProdus);
    });
}

// functia care e apelata pentru editarea unui comentariu

function editeaza(to_edit) {

    var ratingEditareDiv = document.getElementById("reviewEditare_" + to_edit);

    $(ratingEditareDiv).load("/ProdusRating/EditeazaRating/" + to_edit);
}



