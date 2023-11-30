import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { fadeInOut } from 'src/app/fade-in-out';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ListadoMateriales } from 'src/app/models/listadomateriales';
import { Catalogo } from '../../models/catalogo';
import { DetalleMaterialesListadoWidget } from 'src/app/widgets/detallematerialeslistado/detallematerialeslistado.widget';
import { CargarAcuseEntregaWidget } from 'src/app/widgets/cargaracuseentrega/cargaracuseentrega.widget';

@Component({
    selector: 'entrega-comp',
    templateUrl: './entrega.component.html',
    animations: [fadeInOut],
})
export class EntregaComponent {
    model: ListadoMateriales = {
        listas: [], numPaginas: 0, pagina: 1, rows: 0
    }
    meses: Catalogo[];
    tipoListado: Catalogo[]
    mes: number = 0;
    anio: number = 0;
    idEstado: number = 0;
    tipo: number = 0;
    idProveedor: number = 35;
    @ViewChild(DetalleMaterialesListadoWidget, { static: false }) matLis: DetalleMaterialesListadoWidget;
    @ViewChild(CargarAcuseEntregaWidget, { static: false }) acuse: CargarAcuseEntregaWidget;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        http.get<Catalogo[]>(`${url}api/catalogo/obtenermeses`).subscribe(response => {
            this.meses = response;
        })
        http.get<Catalogo[]>(`${url}api/catalogo/obtenertipolistado`).subscribe(response => {
            this.tipoListado = response;
        })
    }
    ngOnInit() {
        const fechaActual = new Date();
        this.anio = fechaActual.getFullYear();
        const fechaActualMes = new Date();
        this.mes = fechaActualMes.getMonth() + 1;
        this.obtenerListados();
    }
    obtenerListados() {
        //this.user.idPersonal
        this.http.get<ListadoMateriales>(`${this.url}api/entrega/obtenerlistados/${this.mes}/${this.anio}/${this.idProveedor}/${this.idEstado}/${this.tipo}/${this.model.pagina}`).subscribe(response => {
            this.model = response;
        }, err => console.log(err));
    }
    goBack() {
        window.history.back();
    }
    muevePagina(event) {
        this.model.pagina = event;
        this.obtenerListados();
    }

    obtenerMateriales(idListado: number, sucursal: string, tipo: string) {
        this.matLis.open(idListado, sucursal, tipo);
    }

    verAcuses(idListado: number, sucursal: string, tipo: string) {
        this.acuse.open(idListado, sucursal, tipo);
    }

}
