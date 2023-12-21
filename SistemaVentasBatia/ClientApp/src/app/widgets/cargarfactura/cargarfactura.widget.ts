import { Component, OnChanges, Output, EventEmitter, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ListadoAcuseEntrega } from '../../models/listadoacuseentrega';
declare var bootstrap: any;
import { Factura } from 'src/app/models/Factura'
import { FacturaComponent } from '../../exclusivo/factura/factura.component';
import { XMLData } from '../../models/xmldata';
import { DetalleOrdenCompra } from '../../models/detalleordencompra';
import { ConfirmaWidget } from '../../widgets/confirma/confirma.widget'
import { XMLGraba } from '../../models/xmlgraba';
import { StoreUser } from 'src/app/stores/StoreUser';
import Swal from 'sweetalert2';

@Component({
    selector: 'cargarfactura-widget',
    templateUrl: './cargarfactura.widget.html'
})
export class CargarFacturaWidget {
    @ViewChild('pdfInput', { static: false }) pdfInput!: ElementRef;
    @ViewChild('xmlInput', { static: false }) xmlInput!: ElementRef;
    @ViewChild(ConfirmaWidget, { static: false }) conwid: ConfirmaWidget;
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
    xmldata: XMLData = {
        subTotal: 0, iva: 0, total: 0, fechaFactura: '', factura: '', uuid: ''
    }
    detallefact: DetalleOrdenCompra = {
        idOrden: 0, idRequisicion: 0, idProveedor: 0, idCliente: 0, proveedor: '', empresa: '', cliente: '', subTotal: 0, iva: 0, total: 0, status: 0, dias: 0, facturado: 0
    }
    fechaActual: Date;
    idTipoFolio: number = null;
    xmlgraba: XMLGraba = {
        factura: '', idCliente: 0, idOrden: 0, idPersonal: 0, fechaFactura: '', dias: 0, subTotal: 0, iva: 0, total: 0, pdfName: '', xmlName: '', uuid: ''
    }
    total: number = 0;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) { }
    nuevo() {
        this.selectedPdf = null;
        this.selectedXml = null;
        this.fechaActual = new Date();
    }
    obtenerDetallesOrden() {
        this.http.get<DetalleOrdenCompra>(`${this.url}api/factura/obtenerdetalleorden/${this.idOrden}`).subscribe(response => {
            this.detallefact = response;
            this.validaOrdenCompleta();
        })
    }

    open(idOrden: number, empresa: string, cliente: string, total: number) {
        this.total = total;
        this.nuevo();
        this.idOrden = idOrden;
        this.empresa = empresa;
        this.cliente = cliente;
        this.obtenerListadoFacturas();
        this.obtenerDetallesOrden();
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
        this.sendEvent.emit(true);
    }

    concluirEntrega() {
    }

    guardarArchivos() {
        this.quitarFocoDeElementos();
    }

    eliminaAcuse(archivo: string, carpeta: string) {
    }

    getDataInsertar() {
        this.xmlgraba.factura = this.xmldata.factura;
        this.xmlgraba.idCliente = this.detallefact.idCliente;
        this.xmlgraba.idOrden = this.detallefact.idOrden;
        this.xmlgraba.idPersonal = this.user.idPersonal;
        this.xmlgraba.fechaFactura = this.xmldata.fechaFactura;
        this.xmlgraba.dias = this.detallefact.dias;
        this.xmlgraba.subTotal = this.xmldata.subTotal;
        this.xmlgraba.iva = this.xmldata.iva;
        this.xmlgraba.total = this.xmldata.total;
        this.xmlgraba.pdfName = this.selectedPdf.name;
        this.xmlgraba.xmlName = this.selectedXml.name;
        this.xmlgraba.uuid = this.xmldata.uuid;
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

    subirFacturas() {
        if (this.selectedPdf && this.selectedXml) {
            this.getDataInsertar();
            const formData = new FormData();
            formData.append('xml', this.selectedXml);
            formData.append('pdf', this.selectedPdf);
            this.http.post<boolean>(`${this.url}api/factura/insertarfacturascarpeta/${this.idOrden}`, formData).subscribe(response => {

            })
            this.http.post<boolean>(`${this.url}api/factura/insertarfacturasxml`, this.xmlgraba).subscribe(response => {
                Swal.fire({
                    icon: 'success',
                    timer: 1000,
                    showConfirmButton: false,
                });
                
                this.obtenerListadoFacturas();
                this.limpiarPDF();
                this.limpiarXML();
                this.obtenerDetallesOrden();
                
            })
        }
    }
    validaOrdenCompleta() {
        if (this.detallefact.facturado == this.total) {
            this.close();
            Swal.fire({
                title: 'Completada',
                text: 'Se cargaron las facturas necesarias',
                icon: 'success',
                timer: 3000,
                showConfirmButton: false,
            });
        }
    }
        
    onPdfSelected(event: any) {
        this.selectedPdf = event.target.files[0];
        this.quitarFocoDeElementos();
    }

    onXmlSelected(event: any) {
        this.selectedXml = event.target.files[0];
        this.quitarFocoDeElementos();
        this.obtenerValoresXML();
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
        this.xmldata = {
            subTotal: 0, iva: 0, total: 0, fechaFactura: '', factura: '', uuid: ''
        }
    }

    obtenerValoresXML() {
        this.http.get<boolean>(`${this.url}api/factura/facturaexiste`)
        if (this.idTipoFolio == null) {
            this.openConfirmacion();
        }
        if (this.selectedXml) {
            const formData = new FormData();
            formData.append('xml', this.selectedXml);
            this.http.post<XMLData>(`${this.url}api/factura/obtenerdatosxml/${this.idTipoFolio}`, formData).subscribe(response => {
                this.xmldata = response;
                this.idTipoFolio = null;
                this.http.get<boolean>(`${this.url}api/factura/facturaexiste/${this.xmldata.uuid}`).subscribe(response => {
                    if (response == true) {
                        this.limpiarXML();
                        Swal.fire({
                            title: 'Error',
                            text: 'Esta factura ya est\u00E1 registrada',
                            icon: 'error',
                            timer: 3000,
                            showConfirmButton: false,
                        });
                    }
                })
            })
        }
    }
    openConfirmacion() {
        this.conwid.titulo = 'Factura';
        this.conwid.mensaje = '\u00BFSu factura cuenta con un folio serializado?'
        this.conwid.open()
    }
    returnConfirmacion($event) {
        if ($event == true) {
            this.idTipoFolio = 1;
        }
        else {
            this.idTipoFolio = 0;
        }
        this.obtenerValoresXML();
    }
}

