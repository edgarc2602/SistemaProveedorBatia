import { Component, OnInit, Inject } from '@angular/core';
import * as Highcharts from 'highcharts';
import { fadeInOut } from 'src/app/fade-in-out';
import { HttpClient } from '@angular/common/http';
import { GraficaListado } from '../../models/graficalistado';
import { Catalogo } from '../../models/catalogo';
import { GraficaOrden } from '../../models/graficaorden';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ListaEvaluacionProveedor } from '../../models/listaevaluacionproveedor';
import { DashOrdenMes } from '../../models/dashordenmes';
import HC_exporting from 'highcharts/modules/exporting';
import { GraficaListadoAnio } from '../../models/graficalistadoanio';
HC_exporting(Highcharts);

import { debounceTime, switchMap } from 'rxjs/operators';
import { Subject } from 'rxjs';


@Component({
    selector: 'dashboard-comp',
    templateUrl: './dashboard.component.html',
    animations: [fadeInOut],
})
export class DashboardComponent implements OnInit {
    graficaListadoMes: GraficaListado = {
        mes: 0, totalListadosPorMes: 0, alta: 0, aprobado: 0, despachado: 0, entregado: 0, cancelado: 0
    }
    graficaListadoAnio: GraficaListadoAnio[] = []

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
    dashOrdenMes: DashOrdenMes = {
        total: 0, facturado: 0
    }
    porcentajeTiempoEntrega: string = '';
    listaAnios: number[] = [];


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {}
    ngOnInit(): void {
        this.idProveedor = this.user.idProveedor;
        const fechaActual = new Date();
        this.anio = fechaActual.getFullYear();
        const fechaActualMes = new Date();
        this.mes = fechaActualMes.getMonth() + 1;
        const anioActual = new Date().getFullYear();
        for (let i = 2018; i <= anioActual + 1; i++) {
            this.listaAnios.push(i);
        }
        this.obtenerMeses();
        this.getPorcentajes();
        this.getGraficas();
        this.getEvaluaciones();
    }
    obtenerMeses() {
        this.http.get<Catalogo[]>(`${this.url}api/catalogo/obtenermeses`).subscribe(response => {
            this.meses = response;
        })
    }

    getPorcentajes() {
        this.http.get<string>(`${this.url}api/usuario/ObtenerEvaluacionTiempoEntrega/${this.anio}/${this.mes}/${this.user.idProveedor}`).subscribe(response => {
            if (response != null) {
                this.porcentajeTiempoEntrega = response;
            }
        })
    }

    getGraficas() {
        this.getGraficasAnio();
        this.getGraficasMes();
    }

    getGraficasAnio() {
        this.graficaListadoAnio = null;
        this.http.get<GraficaListadoAnio[]>(`${this.url}api/usuario/obtenergraficalistadoanio/${this.anio}/${this.idProveedor}`).subscribe(response => {
            if (response.length != 0) {
                this.graficaListadoAnio = response;
                this.getGraficaListadoAnio();
            }
        }, err => console.log(err));
        this.http.get<GraficaOrden[]>(`${this.url}api/usuario/obtenerordenesanio/${this.anio}/${this.idProveedor}`).subscribe(response => {
            if (response.length != 0) {
                this.graficaOrdenAnio = response;
                this.getGraficaOrdenAnio();
            }
        }, err => console.log(err));
    }

    getGraficasMes() {
        this.http.get<GraficaListado>(`${this.url}api/usuario/obtenergraficalistadoaniomes/${this.anio}/${this.mes}/${this.idProveedor}`).subscribe(response => {
            if (response != null) {
                this.graficaListadoMes = response;
            }
        }, err => console.log(err));
        this.http.get<GraficaOrden>(`${this.url}api/usuario/obtenerordenesaniomes/${this.anio}/${this.mes}/${this.idProveedor}`).subscribe(response => {
            if (response != null) {
                this.graficaOrdenMes = response;
            }
        }, err => console.log(err));
        this.http.get<DashOrdenMes>(`${this.url}api/factura/GetDashOrdenMes/${this.user.idProveedor}/${this.anio}/${this.mes}`).subscribe(response => {
            if (response != null) {
                this.dashOrdenMes = response;
            }


        })
        this.getPorcentajes();
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
                y: mes.totalListadosPorMes
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
                    text: 'Listados'
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0">{}Total: </td>' +
                    '<td style="padding:0"><b>{point.y:.0f}</b></td></tr>',
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