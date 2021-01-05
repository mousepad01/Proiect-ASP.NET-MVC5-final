var dropActivateInfo = document.getElementById("dropActivateInfo");
var dropdownContentInfo = document.getElementById("dropdownContentInfo");
var dropdownIconInfo = document.getElementById("dropdownIconInfo");

var clickedInfo = false;

function infoDropdown() {

    if (clickedInfo == false) {

        dropdownContentInfo.style.display = "block";
        dropdownIconInfo.classList = "glyphicon glyphicon-chevron-up";

        clickedInfo = true;
    }
    else {

        dropdownContentInfo.style.display = "none";
        dropdownIconInfo.classList = "glyphicon glyphicon-chevron-down";

        clickedInfo = false;
    }
}

dropActivateInfo.addEventListener("click", infoDropdown);

var dropActivateReview = document.getElementById("dropActivateReview");
var dropdownContentReview = document.getElementById("dropdownContentReview");
var dropdownIconReview = document.getElementById("dropdownIconReview");

var clickedReview = true;

function reviewDropdown() {
    
    if (clickedReview == false) {

        dropdownContentReview.style.display = "block";
        dropdownIconReview.classList = "glyphicon glyphicon-chevron-up";

        clickedReview = true;
    }
    else {

        dropdownContentReview.style.display = "none";
        dropdownIconReview.classList = "glyphicon glyphicon-chevron-down";

        clickedReview = false;
    }
}

dropActivateReview.addEventListener("click", reviewDropdown);

var reviewLink = document.getElementById("reviewLink");


reviewLink.addEventListener("click", reviewDropdown);