
if (document.title == "Produse") {

    var prodLink = document.getElementById("linkProd");
    prodLink.className = "activeLink";
}
else if (document.title == "Categorii") {

    var categLink = document.getElementById("linkCateg");
    categLink.className = "activeLink";
}
else if (document.title == "Adrese") {

    var adrLink = document.getElementById("linkAdrese");
    adrLink.className = "activeLink";
}
else if (document.title == "Register") {

    var regLink = document.getElementById("registerLink");
    regLink.className = "activeLink";
}
else if (document.title == "Log in") {

    var logLink = document.getElementById("loginLink");
    logLink.className = "nav-item active";
}


