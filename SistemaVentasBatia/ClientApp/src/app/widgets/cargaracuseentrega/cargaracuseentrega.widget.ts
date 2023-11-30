import { Component, OnChanges, Output, EventEmitter, SimpleChanges, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DetalleMaterial } from '../../models/detallematerial';
import { AcuseEntrega } from '../../models/acuseentrega';
import { ListadoAcuseEntrega } from '../../models/listadoacuseentrega';
declare var bootstrap: any;

@Component({
    selector: 'cargaracuseentrega-widget',
    templateUrl: './cargaracuseentrega.widget.html'
})
export class CargarAcuseEntregaWidget {
    @Output('ansEvent') sendEvent = new EventEmitter<boolean>();
    model: ListadoAcuseEntrega = {
        acuses: [], carpeta: '', idListado: 0
    }
    sucursal: string;
    tipo: string;
    idListado: number;
    selectedFileName: string | null = null;
    selectedFile: File | null = null;
    public imageUrl: string = '';

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) { }

    nuevo() {
        this.model = {
            acuses: [], carpeta: '', idListado: 0
        }
    }

    open(idListado: number, sucursal: string, tipo: string) {
        this.nuevo();
        this.idListado = idListado;
        this.sucursal = sucursal;
        this.tipo = tipo;
        this.obtenerAcusesListado(idListado);
        let docModal = document.getElementById('modalCargarAcuseEntrega');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }
    obtenerAcusesListado(idListado: number) {
        this.http.get<ListadoAcuseEntrega>(`${this.url}api/entrega/obteneracuseslistado/${idListado}`).subscribe(response => {
            this.model = response;
        })
    }

    acepta() {
        this.sendEvent.emit(true);
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalCargarAcuseEntrega');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    concluirEntrega() {

    }
    verAcuse(archivo: string) {

    }
    eliminaAcuse(archivo: string) {

    }

    limpiarDocumento() {
        this.selectedFileName = null;
    }

    onFileSelected(event: any): void {
        this.selectedFile = event.target.files[0];
        this.selectedFileName = this.selectedFile.name;
    }

    guardarArchivo(): void {
        if (this.selectedFile) {
            const formData = new FormData();
            formData.append('file', this.selectedFile);

            this.http.post<boolean>(`${this.url}api/entrega/guardaracuse`, formData).subscribe((response) => {
                console.log('Archivo guardado con éxito:', response);
            }, (error) => {
                console.error('Error al guardar el archivo:', error);
            });
        } else {
            console.error('No se ha seleccionado ningún archivo.');
        }
    }


    getImage(archivo: string, carpeta: string) {
        this.http.get(`${this.url}api/entrega/getimage/${archivo}/${carpeta}`, { responseType: 'blob' })
            .subscribe((data: Blob) => {
                const reader = new FileReader();
                reader.onload = () => {
                    this.imageUrl = reader.result as string;
                };
                reader.readAsDataURL(data);
            }, error => {
                console.error('Error al obtener la imagen', error);
            });
    }
}

