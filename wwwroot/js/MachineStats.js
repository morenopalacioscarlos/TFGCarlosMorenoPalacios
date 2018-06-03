
window.onload() = function () {
    var pieData = [{ value: 40, color: "#0b82e7", highlight: "#0c62ab", label: "Google Chrome" },
    {
        value: 16,
        color: "#e3e860",
        highlight: "#a9ad47",
        label: "Android"
    },
    {
        value: 11,
        color: "#eb5d82",
        highlight: "#b74865",
        label: "Firefox"
    },
    {
        value: 10,
        color: "#5ae85a",
        highlight: "#42a642",
        label: "Internet Explorer"
    },
    {
        value: 8.6,
        color: "#e965db",
        highlight: "#a6429b",
        label: "Safari"
    }
    ];


    var lineChartData = {
        labels: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
        datasets: [
            {
                label: "Venta Productos",
                fillColor: "rgba(220,220,220,0.2)",
                strokeColor: "#6b9dfa",
                pointColor: "#1e45d7",
                pointStrokeColor: "#fff",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(220,220,220,1)",
                data: [@SoldJanuary, @SoldFebrary, @SoldMarch, @SoldApril, @SoldMay, @SoldJune, @SoldJuly, @SoldAugust, @SoldSeptember, @SoldOctober, @SoldNovember, @SoldDecember]
}
                ]

            }

var graph = document.getElementById("MachineDraw").getContext("2d");
window.myPie = new Chart(graph).Line(lineChartData, { responsive: true });

}
    