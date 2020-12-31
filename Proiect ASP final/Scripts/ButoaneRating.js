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

function editeaza(to_edit, to_edit_rating) {

    var ratingEditareDiv = document.getElementById("reviewEditare_" + to_edit);

    $(ratingEditareDiv).load("/ProdusRating/EditeazaRating/" + to_edit, function () {

        starRating(to_edit, to_edit_rating);
    });
}

//----------------------------------------------------------------------------------------------------------------

var s1Edit, s2Edit, s3Edit, s4Edit, s5Edit, is1Edit, is2Edit, is3Edit, is4Edit, is5Edit;

function starRating(to_edit, to_edit_rating) {

    initEdit(to_edit);

    if (to_edit_rating == 1) {
        mark1Edit();
    }
    else if (to_edit_rating == 2) {
        mark2Edit();
    }
    else if (to_edit_rating == 3) {
        mark3Edit();
    }
    else if (to_edit_rating == 4) {
        mark4Edit();
    }
    else if (to_edit_rating == 5) {
        mark5Edit();
    }
}

function initEdit(editId) {

    s1Edit = document.getElementById("s1Edit_" + editId);
    s2Edit = document.getElementById("s2Edit_" + editId);
    s3Edit = document.getElementById("s3Edit_" + editId);
    s4Edit = document.getElementById("s4Edit_" + editId);
    s5Edit = document.getElementById("s5Edit_" + editId);

    is1Edit = document.getElementById("is1Edit_" + editId);
    is2Edit = document.getElementById("is2Edit_" + editId);
    is3Edit = document.getElementById("is3Edit_" + editId);
    is4Edit = document.getElementById("is4Edit_" + editId);
    is5Edit = document.getElementById("is5Edit_" + editId);

}

function mark1Edit() {

    s1Edit.checked = true;

    is1Edit.classList = "glyphicon glyphicon-star full";
    is2Edit.classList = "glyphicon glyphicon-star";
    is3Edit.classList = "glyphicon glyphicon-star";
    is4Edit.classList = "glyphicon glyphicon-star";
    is5Edit.classList = "glyphicon glyphicon-star";
}

function mark2Edit() {

    s2Edit.checked = true;

    is1Edit.classList = "glyphicon glyphicon-star full";
    is2Edit.classList = "glyphicon glyphicon-star full";
    is3Edit.classList = "glyphicon glyphicon-star";
    is4Edit.classList = "glyphicon glyphicon-star";
    is5Edit.classList = "glyphicon glyphicon-star";
}

function mark3Edit() {

    s3Edit.checked = true;

    is1Edit.classList = "glyphicon glyphicon-star full";
    is2Edit.classList = "glyphicon glyphicon-star full";
    is3Edit.classList = "glyphicon glyphicon-star full";
    is4Edit.classList = "glyphicon glyphicon-star";
    is5Edit.classList = "glyphicon glyphicon-star";
}

function mark4Edit() {

    s4Edit.checked = true;

    is1Edit.classList = "glyphicon glyphicon-star full";
    is2Edit.classList = "glyphicon glyphicon-star full";
    is3Edit.classList = "glyphicon glyphicon-star full";
    is4Edit.classList = "glyphicon glyphicon-star full";
    is5Edit.classList = "glyphicon glyphicon-star";
}

function mark5Edit() {

    s5Edit.checked = true;

    is1Edit.classList = "glyphicon glyphicon-star full";
    is2Edit.classList = "glyphicon glyphicon-star full";
    is3Edit.classList = "glyphicon glyphicon-star full";
    is4Edit.classList = "glyphicon glyphicon-star full";
    is5Edit.classList = "glyphicon glyphicon-star full";
}

function set1Edit(editId) {

    initEdit(editId);

    if (s1Edit.checked == true) {
        mark1Edit();
    }
}

function set2Edit(editId) {

    initEdit(editId);

    if (s2Edit.checked == true) {
        mark2Edit();
    }
}

function set3Edit(editId) {

    initEdit(editId);

    if (s3Edit.checked == true) {
        mark3Edit();
    }
}

function set4Edit(editId) {

    initEdit(editId);

    if (s4Edit.checked == true) {
        mark4Edit();
    }
}

function set5Edit(editId) {

    initEdit(editId);

    if (s5Edit.checked == true) {
        mark5Edit();
    }
}

document.onreadystatechange = function () {
    
    if (document.readyState === 'complete') {

        var modelRatingBufferEdit = document.getElementById("modelRatingBufferEdit");
        var modelIdBufferEdit = document.getElementById("modelIdBufferEdit");
        
        if (modelRatingBufferEdit != null && modelIdBufferEdit != null) {

            var editId = modelIdBufferEdit.value;
            var to_edit_rating = modelRatingBufferEdit.value;

            initEdit(editId);

            if (to_edit_rating == 1) {
                mark1Edit();
            }
            else if (to_edit_rating == 2) {
                mark2Edit();
            }
            else if (to_edit_rating == 3) {
                mark3Edit();
            }
            else if (to_edit_rating == 4) {
                mark4Edit();
            }
            else if (to_edit_rating == 5) {
                mark5Edit();
            }
        }
    }
}

    




///---------------------------------------------------------------------------

var s1, s2, s3, s4, s5, is1, is2, is3, is4, is5;

// verific daca butonul este pe pagina. daca nu, inseamna ca form ul de adaugare deja este deschis (modelul a intors un mesaj de eroare)
// in acel caz verific valoarea de rating intoarsa de model
if (ratingNouBtn == null) {

    var modelRatingBuffer = document.getElementById("modelRatingBuffer");

    init();

    if (modelRatingBuffer != null) {

        if (+modelRatingBuffer.value == 1) {
            mark1();
        }
        else if (+modelRatingBuffer.value == 2) {
            mark2();
        }
        else if (+modelRatingBuffer.value == 3) {
            mark3();
        }
        else if (+modelRatingBuffer.value == 4) {
            mark4();
        }
        else if (+modelRatingBuffer.value == 5) {
            mark5();
        }
    }
}

function init() {

    s1 = document.getElementById("s1");
    s2 = document.getElementById("s2");
    s3 = document.getElementById("s3");
    s4 = document.getElementById("s4");
    s5 = document.getElementById("s5");

    is1 = document.getElementById("is1");
    is2 = document.getElementById("is2");
    is3 = document.getElementById("is3");
    is4 = document.getElementById("is4");
    is5 = document.getElementById("is5");
}

function mark1() {

    s1.checked = true;

    is1.classList = "glyphicon glyphicon-star full";
    is2.classList = "glyphicon glyphicon-star";
    is3.classList = "glyphicon glyphicon-star";
    is4.classList = "glyphicon glyphicon-star";
    is5.classList = "glyphicon glyphicon-star";
}

function mark2() {

    s2.checked = true;

    is1.classList = "glyphicon glyphicon-star full";
    is2.classList = "glyphicon glyphicon-star full";
    is3.classList = "glyphicon glyphicon-star";
    is4.classList = "glyphicon glyphicon-star";
    is5.classList = "glyphicon glyphicon-star";
}

function mark3() {

    s3.checked = true;

    is1.classList = "glyphicon glyphicon-star full";
    is2.classList = "glyphicon glyphicon-star full";
    is3.classList = "glyphicon glyphicon-star full";
    is4.classList = "glyphicon glyphicon-star";
    is5.classList = "glyphicon glyphicon-star";
}

function mark4() {

    s4.checked = true;

    is1.classList = "glyphicon glyphicon-star full";
    is2.classList = "glyphicon glyphicon-star full";
    is3.classList = "glyphicon glyphicon-star full";
    is4.classList = "glyphicon glyphicon-star full";
    is5.classList = "glyphicon glyphicon-star";
}

function mark5() {

    s5.checked = true;

    is1.classList = "glyphicon glyphicon-star full";
    is2.classList = "glyphicon glyphicon-star full";
    is3.classList = "glyphicon glyphicon-star full";
    is4.classList = "glyphicon glyphicon-star full";
    is5.classList = "glyphicon glyphicon-star full";
}

function set1() {

    init();

    if (s1.checked == true) {
        mark1();
    }
}

function set2() {

    init();

    if (s2.checked == true) {
        mark2();
    }
}

function set3() {

    init();

    if (s3.checked == true) {
        mark3();
    }
}

function set4() {

    init();

    if (s4.checked == true) {
        mark4();
    }
}

function set5() {

    init();

    if (s5.checked == true) {
        mark5();
    }
}

//-------------------------------------------------------------------------------------------



