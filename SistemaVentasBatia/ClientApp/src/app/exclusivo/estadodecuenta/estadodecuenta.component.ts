import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { fadeInOut } from 'src/app/fade-in-out';


import { StoreUser } from 'src/app/stores/StoreUser';
import { UsuarioRegistro } from 'src/app/models/usuarioregistro';
import { EstadoDeCuenta } from '../../models/estadodecuenta';
import { ListadoEstadoDeCuenta } from '../../models/listadoestadodecuenta';
import { ListaEvaluacionProveedor } from '../../models/listaevaluacionproveedor';

@Component({
    selector: 'estadodecuenta-comp',
    templateUrl: './estadodecuenta.component.html',
    animations: [fadeInOut],
})
export class EstadoDeCuentaComponent {
    usuario: UsuarioRegistro = {
        idAutorizacionVentas: 0, idPersonal: 0, autoriza: 0, nombres: '', apellidos: '', puesto: '', telefono: '', telefonoExtension: '', telefonoMovil: '', email: '',
        firma: '', revisa: 0
    }
    model: ListadoEstadoDeCuenta = {
        estadosDeCuenta: [], numPaginas: 0, pagina: 1, rows: 0
    }
    listaEvaluaciones: ListaEvaluacionProveedor = {
        evaluacion: [], idEvaluacionProveedor: 0,idProveedor: 0, idStatus: 0, fechaEvaluacion: '', numeroContrato: '', promedio: 0, textoPromedio: '', idUsuario: 0
    }
    totalMontoFactura: number = 0;
    totalPagos: number = 0;
    totalSaldoActual: number = 0;
    totalCorriente: number = 0;
    totalPrimerMes: number = 0;
    totalSegundoMes: number = 0;
    totalTercerMes: number = 0;
    totalCuartoMesOMas: number= 0


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
    }
    goBack() {
        window.history.back();
    }
    ngOnInit() {
        this.http.get<ListadoEstadoDeCuenta>(`${this.url}api/cuenta/getestadodecuenta/${this.user.idProveedor}/${this.model.pagina}`).subscribe(response => {
            this.model = response;
            this.limpiarTotales();
            this.model.estadosDeCuenta.forEach(estado => {
                this.totalMontoFactura += estado.total;
                this.totalPagos += estado.pago;
                this.totalSaldoActual += estado.saldo;
                this.totalCorriente += estado.corriente;
                this.totalPrimerMes += estado.mes1;
                this.totalSegundoMes += estado.mes2;
                this.totalTercerMes += estado.mes3;
                this.totalCuartoMesOMas += estado.mes4;
            })

        })
        
    }
    limpiarTotales() {
        this.totalMontoFactura = 0;
        this.totalPagos = 0;
        this.totalSaldoActual = 0;
        this.totalCorriente = 0;
        this.totalPrimerMes = 0;
        this.totalSegundoMes = 0;
        this.totalTercerMes = 0;
        this.totalCuartoMesOMas = 0;
    }

    muevePagina(event) {
        this.model.pagina = event;
        this.ngOnInit();
    }

    
}