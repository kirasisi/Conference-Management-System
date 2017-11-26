$(document).ready(function () {
    $('#table').dataTable({ "sDom": '<"nav"lf>t<"nav"i>' });
    $('#table tbody tr:even').addClass("silver");
});
