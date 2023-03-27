// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
if (localStorage.getItem("viewPreference") === "list") {
    list();
}

function list() {
    localStorage.setItem("viewPreference", "list");
    document.querySelector("#grid").classList.add("d-none");
    document.querySelector("#list").classList.remove("d-none");
}

function grid() {
    localStorage.setItem("viewPreference", "grid");
    document.querySelector("#list").classList.remove("d-none");
    document.querySelector("#list").classList.add("d-none");
}

$("#filterForm input[name=SearchInput]").on('change', (e) => filterMission(e))
function filterMission(e) {
    e.preventDefault();
    console.log("Searching..")
}