import { Component, OnInit, Inject } from '@angular/core';
import * as Highcharts from 'highcharts';
import { fadeInOut } from 'src/app/fade-in-out';
import { HttpClient } from '@angular/common/http';
import { GraficaListado } from '../../models/graficalistado';
import { Catalogo } from '../../models/catalogo';
import { GraficaOrden } from '../../models/graficaorden';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ListaEvaluacionProveedor } from '../../models/listaevaluacionproveedor';


@Component({
    selector: 'dashboard-comp',
    templateUrl: './dashboard.component.html',
    animations: [fadeInOut],
})
export class DashboardComponent implements OnInit {
    graficaListadoMes: GraficaListado = {
        mes: 0, totalListadosPorMes: 0, alta: 0, aprobado: 0, despachado: 0, entregado: 0, cancelado: 0
    }
    graficaListadoAnio: GraficaListado[] = []

    graficaOrdenMes: GraficaOrden = {
        mes: 0, totalOrdenesPorMes: 0, alta: 0, autorizada: 0, rechazada: 0, completa: 0, despachada: 0, enRequicision: 0
    }
    graficaOrdenAnio: GraficaOrden[] = []
    mes: number = 0;
    anio: number = 0;
    idProveedor: number = 0;
    meses: Catalogo[];

    listaEvaluaciones: ListaEvaluacionProveedor = {
        evaluacion: [], idEvaluacionProveedor: 0, idProveedor: 0, idStatus: 0, fechaEvaluacion: '', numeroContrato: '', promedio: 0, textoPromedio: '', idUsuario: 0
    }

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        http.get<Catalogo[]>(`${url}api/catalogo/obtenermeses`).subscribe(response => {
            this.meses = response;
        })
    }
    ngOnInit(): void {
        this.idProveedor = this.user.idProveedor;
        const fechaActual = new Date();
        this.anio = fechaActual.getFullYear();
        const fechaActualMes = new Date();
        this.mes = fechaActualMes.getMonth() + 1;
        this.getGraficas();
        this.getEvaluaciones();
    }

    getGraficas() {
        this.graficaListadoAnio = null;
        this.graficaListadoMes = null;
        this.http.get<GraficaListado[]>(`${this.url}api/usuario/obtenergraficalistadoanio/${this.anio}/${this.idProveedor}`).subscribe(response => {
            this.graficaListadoAnio = response;
            this.getGraficaListadoAnio();
        }, err => console.log(err));
        this.http.get<GraficaListado>(`${this.url}api/usuario/obtenergraficalistadoaniomes/${this.anio}/${this.mes}/${this.idProveedor}`).subscribe(response => {
            this.graficaListadoMes = response;
            this.getGraficaListadoMes();
        }, err => console.log(err));
        this.http.get<GraficaOrden[]>(`${this.url}api/usuario/obtenerordenesanio/${this.anio}/${this.idProveedor}`).subscribe(response => {
            this.graficaOrdenAnio = response;
            this.getGraficaOrdenAnio();
        }, err => console.log(err));
        this.http.get<GraficaOrden>(`${this.url}api/usuario/obtenerordenesaniomes/${this.anio}/${this.mes}/${this.idProveedor}`).subscribe(response => {
            this.graficaOrdenMes = response;
            this.getGraficaOrdenMes();
        }, err => console.log(err));
    }

    getGraficaListadoMes() {
        let container: HTMLElement = document.getElementById('glm');
        const categories = ['Alta', 'Aprobado', 'Despachado', 'Entregado', 'Cancelado'];
        const data = [
            this.graficaListadoMes.alta,
            this.graficaListadoMes.aprobado,
            this.graficaListadoMes.despachado,
            this.graficaListadoMes.entregado,
            this.graficaListadoMes.cancelado
        ];

        Highcharts.chart(container, {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Mensual'
            },
            subtitle: {
                text: `Total: ${this.graficaListadoMes.totalListadosPorMes}`,
                align: 'center',
                style: {
                    fontSize: '16px'
                }
            },
            xAxis: {
                categories: categories,
                crosshair: true
            },
            yAxis: {
                allowDecimals: false,
                min: 0,
                title: {
                    text: ''
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                formatter: function () {
                    return '<b>' + this.x + '</b><br/>' +
                        'Total: ' + this.y + '<br/>';
                },
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: [{
                type: 'column',
                name: 'Detalles',
                data: data
            }]
        });
    }

    getGraficaListadoAnio() {
        let container: HTMLElement = document.getElementById('gla');
        const meses = [
            'Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'
        ];
        let total = 0;
        const seriesData = this.graficaListadoAnio.map(mes => {
            total += mes.totalListadosPorMes;
            return {
                name: meses[mes.mes - 1],
                y: mes.totalListadosPorMes,
                alta: mes.alta,
                aprobado: mes.aprobado,
                despachado: mes.despachado,
                entregado: mes.entregado,
                cancelado: mes.cancelado
            };
        });
        const totalSubtitle = `Total: ${total}`;

        Highcharts.chart(container, {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Anual'
            },
            subtitle: {
                text: totalSubtitle,
                align: 'center',
                style: {
                    fontSize: '16px'
                }
            },
            xAxis: {
                categories: meses,
                crosshair: true
            },
            yAxis: {
                allowDecimals: false,
                min: 0,
                title: {
                    text: 'Listados'
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0">{}Total: </td>' +
                    '<td style="padding:0"><b>{point.y:.0f}</b></td></tr>',
                    //+ '<tr><td>Alta:</td><td>{point.alta}</td></tr>' +
                    //'<tr><td>Aprobado:</td><td>{point.aprobado}</td></tr>' +
                    //'<tr><td>Despachado:</td><td>{point.despachado}</td></tr>' +
                    //'<tr><td>Entregado:</td><td>{point.entregado}</td></tr>' +
                    //'<tr><td>Cancelado:</td><td>{point.cancelado}</td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            series: [{
                type: 'column',
                name: 'Meses',
                data: seriesData
            }]
        });
    }

    getGraficaOrdenMes() {
        let container: HTMLElement = document.getElementById('gom');
        const categories = ['Alta', 'Autorizada', 'Rechazada', 'Completa', 'Despachada', 'En Requisicion'];
        const data = [
            this.graficaOrdenMes.alta,
            this.graficaOrdenMes.autorizada,
            this.graficaOrdenMes.rechazada,
            this.graficaOrdenMes.completa,
            this.graficaOrdenMes.despachada,
            this.graficaOrdenMes.enRequicision
        ];

        Highcharts.chart(container, {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Mensual'
            },
            subtitle: {
                text: `Total: ${this.graficaOrdenMes.totalOrdenesPorMes}`,
                align: 'center',
                style: {
                    fontSize: '16px'
                }
            },
            xAxis: {
                categories: categories,
                crosshair: true
            },
            yAxis: {
                allowDecimals: false,
                min: 0,
                title: {
                    text: ''
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                formatter: function () {
                    return '<b>' + this.x + '</b><br/>' +
                        'Total: ' + this.y + '<br/>';
                },
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    color: '#5094fc',
                    borderWidth: 0
                }
            },
            series: [{
                type: 'column',
                name: 'Detalles',
                data: data
            }]
        });
    }

    getGraficaOrdenAnio() {
        let container: HTMLElement = document.getElementById('goa');
        const meses = [
            'Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'
        ];
        let totalOrden = 0;
        const seriesData = this.graficaOrdenAnio.map(mes => {
            totalOrden += mes.totalOrdenesPorMes;
            return {
                name: meses[mes.mes - 1],
                y: mes.totalOrdenesPorMes,
                alta: mes.alta,
                aprobado: mes.autorizada,
                despachado: mes.rechazada,
                entregado: mes.completa,
                cancelado: mes.despachada
            };
        });
        const totalSubtitle = `Total: ${totalOrden}`;

        Highcharts.chart(container, {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Anual'
            },
            subtitle: {
                text: totalSubtitle,
                align: 'center',
                style: {
                    fontSize: '16px'
                }
            },
            plotOptions: {
                column: {
                    color: '#5094fc',
                }
            },
            xAxis: {
                categories: meses,
                crosshair: true
            },
            yAxis: {
                allowDecimals: false,
                min: 0,
                title: {
                    text: 'Ordenes'
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0">{}Total: </td>' +
                    '<td style="padding:0"><b>{point.y:.0f}</b></td></tr>',
                //+'<tr><td>Alta:</td><td>{point.alta}</td></tr>' +
                //'<tr><td>Aprobado:</td><td>{point.aprobado}</td></tr>' +
                //'<tr><td>Despachado:</td><td>{point.despachado}</td></tr>' +
                //'<tr><td>Entregado:</td><td>{point.entregado}</td></tr>' +
                //'<tr><td>Cancelado:</td><td>{point.cancelado}</td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            series: [{
                type: 'column',
                name: 'Meses',
                data: seriesData
            }]
        });
    }
   
    goBack() {
        window.history.back();
    }

    getEvaluaciones() {
        this.http.get<ListaEvaluacionProveedor>(`${this.url}api/cuenta/GetListadoEvaluacionProveedor/${this.user.idProveedor}`).subscribe(response => {
            this.listaEvaluaciones = response;
        })
    }

    filtroPlan(id: number) {
        if (this.listaEvaluaciones && Array.isArray(this.listaEvaluaciones) && this.listaEvaluaciones.length > 0) {
            let list = this.listaEvaluaciones.filter(item =>
                item.evaluacion && Array.isArray(item.evaluacion) && item.evaluacion.some(evaluacion =>
                    evaluacion.idEvaluacionProveedor === id
                )
            );
            return list.length > 0 ? list[0].evaluacion : [];
        } else {
            return [];
        }
    }

    quitarfoco() {
        this.quitarFocoDeElementos();
    }

    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}