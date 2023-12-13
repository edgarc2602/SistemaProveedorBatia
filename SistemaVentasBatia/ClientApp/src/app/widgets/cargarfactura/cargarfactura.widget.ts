import { Component, OnChanges, Output, EventEmitter, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ListadoAcuseEntrega } from '../../models/listadoacuseentrega';
declare var bootstrap: any;
import { Factura } from 'src/app/models/Factura'
import { FacturaComponent } from '../../exclusivo/factura/factura.component';

@Component({
    selector: 'cargarfactura-widget',
    templateUrl: './cargarfactura.widget.html'
})
export class CargarFacturaWidget {
    @ViewChild('pdfInput', { static: false }) pdfInput!: ElementRef;
    @ViewChild('xmlInput', { static: false }) xmlInput!: ElementRef;
    @Output('ansEvent') sendEvent = new EventEmitter<boolean>();
    model: ListadoAcuseEntrega = {
        acuses: [], carpeta: '', idListado: 0
    }
    sucursal: string;
    tipo: string;
    idListado: number;
    public imageUrl: string = '';
    formato: string = '';
    prefijo: string = '';

    idOrden: number = 0;
    empresa: string = '';
    cliente: string = '';
    selectedPdf: File | null = null;
    selectedXml: File | null = null;
    facturas: Factura = {} as Factura;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) { }
    nuevo() {
        /*this.resetFileInput();*/
    }

    open(idOrden: number, empresa: string, cliente: string) {
        this.nuevo();
        this.idOrden = idOrden;
        this.empresa = empresa;
        this.cliente = cliente;
        this.selectedPdf = null;
        this.selectedXml = null;
        this.obtenerListadoFacturas();
        let docModal = document.getElementById('modalCargarFactura');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    acepta() {
        this.sendEvent.emit(true);
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalCargarFactura');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    concluirEntrega() {

    }
    guardarArchivos() {
        //if (this.selectedFile) {
        //    const formData = new FormData();
        //    formData.append('file', this.selectedFile);

        //    this.http.post<boolean>(`${this.url}api/entrega/guardaracuse/${this.idListado}/${this.selectedFileName}`, formData).subscribe((response) => {
        //        console.log('Archivo guardado con éxito:', response);
        //        this.selectedFileName = null;
        //        this.selectedFile = null;
        //        //this.obtenerAcusesListado(this.idListado);
        //        this.resetFileInput();
        //    }, (error) => {
        //        console.error('Error al guardar el archivo:', error);
        //    });
        //} else {
        //    console.error('No se ha seleccionado ningún archivo.');
        //}
        this.quitarFocoDeElementos();
    }
    eliminaAcuse(archivo: string, carpeta: string) {
        //this.http.delete<boolean>(`${this.url}api/entrega/eliminaacuse/${archivo}/${carpeta}/${this.idListado}`).subscribe(response => {
        //    console.log('Archivo eliminado con éxito:', response);
        //    this.selectedFileName = null;
        //    this.selectedFile = null;
        //    //this.obtenerAcusesListado(this.idListado);
        //    this.resetFileInput();
        //}, (error) => {
        //    console.error('Error al eliminar el archivo:', error);
        //});

    }

    

    openDocument(archivo: string, carpeta: string) {
        this.getImage(archivo, carpeta);
    }
    getImage(archivo: string, carpeta: string) {
        this.http.get(`${this.url}api/entrega/getimage/${archivo}/${carpeta}`, { responseType: 'blob' })
            .subscribe((data: Blob) => {
                const extension = this.obtenerExtension(archivo);
                switch (extension) {
                    case 'pdf':
                        this.formato = 'application/pdf'
                        break;
                    case 'jpeg':
                        this.formato = 'image/jpeg'
                        break;
                    case 'jpg':
                        this.formato = 'image/jpg'
                        break;
                    case 'png':
                        this.formato = 'image/png'
                        break;
                    case 'PDF':
                        this.formato = 'application/pdf'
                        break;
                    case 'JPEG':
                        this.formato = 'image/jpeg'
                        break;
                    case 'JPG':
                        this.formato = 'image/jpg'
                        break;
                    case 'PNG':
                        this.formato = 'image/png'
                        break;
                    default:
                        break;
                }
                const file = new Blob([data], { type: this.formato });
                const fileURL = URL.createObjectURL(file);
                const width = 800;
                const height = 550;
                const left = window.innerWidth / 2 - width / 2;
                const top = window.innerHeight / 2 - height / 2;
                const newWindow = window.open(fileURL, '_blank', `width=${width}, height=${height}, top=${top}, left=${left}`);
                if (newWindow) {
                    newWindow.focus();
                } else {
                    alert('La ventana emergente ha sido bloqueada. Por favor, permite ventanas emergentes para este sitio.');
                }
            }, error => {
                console.error('Error al obtener el documento', error);
            });
    }
    obtenerExtension(archivo: string): string {
        const partes = archivo.split('.');
        const extension = partes[partes.length - 1];
        return extension;
    }

    
    //enviar facturas al Backend
    subirFacturas() {
        if (this.selectedPdf && this.selectedXml) {
            const formData = new FormData();
            formData.append('xml', this.selectedXml);
            formData.append('pdf', this.selectedPdf);
            this.http.post<boolean>(`${this.url}api/factura/insertarfacturas/${this.idOrden}`, formData).subscribe(response => {
                
            })
        }
    }
    //apartado para seleccionar documentacion

    onPdfSelected(event: any) {
        this.selectedPdf = event.target.files[0];
        this.quitarFocoDeElementos();
    }

    onXmlSelected(event: any) {
        this.selectedXml = event.target.files[0];
        this.quitarFocoDeElementos();
    }

    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');

        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }

    obtenerListadoFacturas() {
         this.http.get<Factura>(`${this.url}api/factura/obtenerfacturas/${this.idOrden}`).subscribe(response => {
            this.facturas = response;
        })
    }

    limpiarPDF() {
        this.selectedPdf = null;
        this.pdfInput.nativeElement.value = '';
    }
    limpiarXML() {
        this.selectedXml = null;
        this.xmlInput.nativeElement.value = '';
    }
}

