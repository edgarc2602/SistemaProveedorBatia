import { Component, OnChanges, Output, EventEmitter, Inject, ViewChild, ElementRef, SimpleChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Resultado } from '../../models/resultado';
import Swal from 'sweetalert2';
import { StoreUser } from '../../stores/StoreUser';
import { User } from 'oidc-client';
import { OrdenCompraDetalle } from '../../models/ordenCompraDetalle';
import { OrdenCompra } from '../../models/ordencompra';
import { ConceptoCFDI, EmisorCFDI, FacturaCFDI, ImpuestosCFDI, ImpuestosConceptoCFDI, ReceptorCFDI, RetencionConceptoCFDI, RetencionGlobalCFDI, TimbreFiscalCFDI, TrasladoCFDI } from '../../models/FacturaCFDI';
import { Recepcion } from '../../models/Recepcion';
import { parseString } from 'xml2js';
declare var bootstrap: any;

@Component({
    selector: 'requisicionOrdenCompraDetalleFactura-widget',
    templateUrl: './requisicionOrdenCompraDetalleFactura.widget.html'
})
export class RequisicionOrdenCompraDetalleFacturaWidget {
    [x: string]: any;
    @ViewChild('pdfInput', { static: false }) pdfInput!: ElementRef;
    @ViewChild('xmlInput', { static: false }) xmlInput!: ElementRef;
    @Output('supEvent') sendEvent = new EventEmitter<boolean>();

    idOrdenCompra: number = 0;
    ingresaInventario: number = 0;

    selectedPdf: File | null = null;
    selectedXml: File | null = null;

    modelFacturaCFDI: FacturaCFDI = {
        version: "",
        fecha: "",
        serie: "",
        folio: "",
        subtotal: "",
        descuento: "",
        total: "",
        formaPago: "",
        metodoPago: "",
        lugarExpedicion: "",

        emisor: {
            rfc: "",
            nombre: "",
            regimenFiscal: ""
        },

        receptor: {
            rfc: "",
            nombre: "",
            usoCFDI: "",
            regimenFiscalReceptor: "",
            domicilioFiscal: ""
        },

        conceptos: [],

        impuestos: {
            totalTrasladados: "",
            totalRetenidos: "",
            traslados: []
        },

        timbreFiscal: {
            uuid: "",
            fechaTimbrado: "",
            rfcProvCertif: "",
            selloCFD: "",
            version: "",
            noCertificadoSAT: "",
            selloSAT: ""
        }
    };

    modelOrdenCompra: OrdenCompra = {
        idOrden: 0,
        tipo: "",
        estatus: "",
        fechaAlta: "",
        empresa: "",
        proveedor: "",
        idCliente: 0,
        cliente: "",
        elabora: "",
        observacion: "",
        inventario: 0,
        facturado: 0,
        idCredito: 0,
        credito: "",
        diasCredito: 0,
        idAlmacen: 0,
        loading: false,
        iva: 0,
        subTotal: 0,
        total: 0
    };

    modelOrdenCompraDetalle: OrdenCompraDetalle = {
        idOrden: 0,
        productos: []
    }

    modelResultado: Resultado = {
        estatus: false,
        mensaje: "",
        mensajeError: "",
        objeto: "",
        objetos: []
    }

    loading: boolean = false;
    isLoading: boolean = false;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) { }

    // ******** Funciones del componente
    ngOnInit() {
        this.limpiaModelo();
    }

    ngOnChanges(changes: SimpleChanges): void { }


    // ******** Funciones para acciones del modal
    regresaSup() { }

    open(idOrdenCompra: number, ingresaInventario: number, modelOrdenCompra: OrdenCompra, modelOrdenCompraDetalle: OrdenCompraDetalle): void {
        let docModal = document.getElementById('modalRequisicionOrdenCompraDetalleFactura');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        this.idOrdenCompra = idOrdenCompra;
        this.ingresaInventario = ingresaInventario;
        this.modelOrdenCompra = modelOrdenCompra;
        this.modelOrdenCompraDetalle = modelOrdenCompraDetalle;
        console.log("ingresa al inventario: " + this.ingresaInventario)
        myModal.show();
    }

    acepta() {
        this.sendEvent.emit(true);
        this.close();
    }

    cancela() {
        this.sendEvent.emit(false);
        this.close();
    }

    close() {
        this.limpiarPDF();
        this.limpiarXML();
        let docModal = document.getElementById('modalPresentacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }


    // ******** Funciones para mostrar datos
    cargaOrdenCompra(): void {
        this.isLoading = true;
        this.http.get<Resultado>(`${this.url}api/ordenCompra/getordencompra/${this.idOrdenCompra}`).subscribe(response => {
            console.log(response);
            this.modelResultado = response;

            if (this.modelResultado.estatus == true) {

            } else {
                console.log(this.modelResultado.mensaje);
                Swal.fire({
                    title: 'Error',
                    text: 'Ocurrio un error al consultar el detalle de la orden de compra de la requisicion',
                    icon: 'error',
                    timer: 3000,
                    showConfirmButton: false,
                });
            }

            this.isLoading = false;
        }, err => {
            Swal.fire({
                title: 'Error',
                text: 'Ocurrio un error al consultar el detalle de la requisicion',
                icon: 'error',
                timer: 3000,
                showConfirmButton: false,
            });
        });
    }

    cargaOrdenCompraDetalle(): void {
        this.isLoading = true;
        this.http.get<Resultado>(`${this.url}api/ordenCompra/getordencompradetalle/${this.idOrdenCompra}`).subscribe(response => {
            console.log(response);
            this.modelResultado = response;

            if (this.modelResultado.estatus == true) {

            } else {
                console.log(this.modelResultado.mensaje);
                Swal.fire({
                    title: 'Error',
                    text: 'Ocurrio un error al consultar el detalle de la orden de compra de la requisicion',
                    icon: 'error',
                    timer: 3000,
                    showConfirmButton: false,
                });
            }

            this.isLoading = false;
        }, err => {
            Swal.fire({
                title: 'Error',
                text: 'Ocurrio un error al consultar el detalle de la requisicion',
                icon: 'error',
                timer: 3000,
                showConfirmButton: false,
            });
        });
    }

    // ******** Funciones para mapeo de datos
    mapearFactura(json: any): FacturaCFDI {
        const c = json['cfdi:Comprobante'];

        return {
            version: c.$.Version,
            fecha: c.$.Fecha,
            sello: c.$.Sello,
            noCertificado: c.$.NoCertificado,
            certificado: c.$.Certificado,
            condicionesDePago: c.$.CondicionesDePago,

            serie: c.$.Serie,
            folio: c.$.Folio,

            formaPago: c.$.FormaPago,
            metodoPago: c.$.MetodoPago,
            tipoDeComprobante: c.$.TipoDeComprobante,
            exportacion: c.$.Exportacion,
            moneda: c.$.Moneda,

            subtotal: c.$.SubTotal,
            descuento: c.$.Descuento,
            total: c.$.Total,
            lugarExpedicion: c.$.LugarExpedicion,

            emisor: this.mapearEmisor(c['cfdi:Emisor']),
            receptor: this.mapearReceptor(c['cfdi:Receptor']),

            conceptos: this.mapearConceptos(c['cfdi:Conceptos']),

            impuestos: this.mapearImpuestos(c['cfdi:Impuestos']),

            timbreFiscal: this.mapearTimbre(c['cfdi:Complemento'])
        };
    }

    mapearEmisor(e: any): EmisorCFDI {
        return {
            rfc: e.$.Rfc,
            nombre: e.$.Nombre,
            regimenFiscal: e.$.RegimenFiscal
        };
    }

    mapearReceptor(r: any): ReceptorCFDI {
        return {
            rfc: r.$.Rfc,
            nombre: r.$.Nombre,
            usoCFDI: r.$.UsoCFDI,
            regimenFiscalReceptor: r.$.RegimenFiscalReceptor,
            domicilioFiscal: r.$.DomicilioFiscalReceptor
        };
    }

    mapearConceptos(cs: any): ConceptoCFDI[] {
        const conceptos = cs['cfdi:Concepto'];

        // Si solo viene 1 concepto, xml2js NO lo convierte en array
        const lista = Array.isArray(conceptos) ? conceptos : [conceptos];

        return lista.map((c: any) => ({
            claveProdServ: c.$.ClaveProdServ,
            noIdentificacion: c.$.NoIdentificacion,
            cantidad: c.$.Cantidad,
            claveUnidad: c.$.ClaveUnidad,
            unidad: c.$.Unidad,
            descripcion: c.$.Descripcion,
            valorUnitario: c.$.ValorUnitario,
            importe: c.$.Importe,
            descuento: c.$.Descuento,
            objetoImp: c.$.ObjetoImp,

            impuestos: this.mapearImpuestosConcepto(c['cfdi:Impuestos']),
            cuentaPredial: c['cfdi:CuentaPredial']
                ? { numero: c['cfdi:CuentaPredial'].$.Numero }
                : undefined
        }));
    }

    mapearImpuestosConcepto(i: any): ImpuestosConceptoCFDI | undefined {
        if (!i) return undefined;

        return {
            traslados: i['cfdi:Traslados']
                ? this.mapearTraslados(i['cfdi:Traslados']['cfdi:Traslado'])
                : undefined,

            retenciones: i['cfdi:Retenciones']
                ? this.mapearRetencionesConcepto(i['cfdi:Retenciones']['cfdi:Retencion'])
                : undefined
        };
    }

    mapearImpuestos(i: any): ImpuestosCFDI | undefined {
        if (!i) return undefined;

        return {
            totalTrasladados: i.$ ? i.$.TotalImpuestosTrasladados : undefined,
            totalRetenidos: i.$ ? i.$.TotalImpuestosRetenidos : undefined,
            traslados: i['cfdi:Traslados']
                ? this.mapearTraslados(i['cfdi:Traslados']['cfdi:Traslado'])
                : undefined,

            retenciones: i['cfdi:Retenciones']
                ? this.mapearRetencionesGlobales(i['cfdi:Retenciones']['cfdi:Retencion'])
                : undefined
        };
    }

    mapearTraslados(items: any): TrasladoCFDI[] {
        const arr = Array.isArray(items) ? items : [items];

        return arr.map(t => ({
            base: t.$.Base,
            impuesto: t.$.Impuesto,
            tipoFactor: t.$.TipoFactor,
            tasaOCuota: t.$.TasaOCuota,
            importe: t.$.Importe
        }));
    }

    mapearRetencionesConcepto(items: any): RetencionConceptoCFDI[] {
        const arr = Array.isArray(items) ? items : [items];

        return arr.map(r => ({
            base: r.$.Base,
            impuesto: r.$.Impuesto,
            tipoFactor: r.$.TipoFactor,
            tasaOCuota: r.$.TasaOCuota,
            importe: r.$.Importe
        }));
    }

    mapearRetencionesGlobales(items: any): RetencionGlobalCFDI[] {
        const arr = Array.isArray(items) ? items : [items];

        return arr.map(r => ({
            impuesto: r.$.Impuesto,
            importe: r.$.Importe
        }));
    }

    mapearTimbre(complemento: any): TimbreFiscalCFDI | undefined {
        if (!complemento) return undefined;

        const t = complemento['tfd:TimbreFiscalDigital'] &&
            complemento['tfd:TimbreFiscalDigital'].$
            ? complemento['tfd:TimbreFiscalDigital'].$
            : null;

        if (!t) return undefined;

        return {
            version: t.version,
            uuid: t.UUID,
            fechaTimbrado: t.FechaTimbrado,
            rfcProvCertif: t.RfcProvCertif,
            selloCFD: t.SelloCFD,
            noCertificadoSAT: t.NoCertificadoSAT,
            selloSAT: t.SelloSAT
        };
    }

    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }

    verificaPDF(event: any) {
        this.selectedPdf = event.target.files[0];
        this.quitarFocoDeElementos();
    }

    // ******** Funciones para validar
    verificaFacturaCFDI(event: any) {
        this.selectedXml = event.target.files[0];

        this.isLoading = true;
        const file: File = event.target.files[0];

        // Sale de la funcion si no se cargo ningun documento
        if (!file) return;

        // Valida que el documento subido sea un xml
        if (file.type !== "text/xml" && !file.name.endsWith(".xml")) {
            Swal.fire({
                title: 'Advertencia',
                text: 'Solo se permiten archivos XML',
                icon: 'error',
                showConfirmButton: false,
            });
            (event.target as HTMLInputElement).value = '';
            return false;
        }

        const reader = new FileReader();

        reader.onload = () => {
            const xmlContent = reader.result as string;

            parseString(xmlContent, { explicitArray: false }, (err, result) => {
                if (err) {
                    Swal.fire({
                        title: 'Error',
                        text: 'Ocurrio un error al intentar leer la Factura CFDI. Detalle: ' + err,
                        icon: 'error',
                        showConfirmButton: false,
                    });
                    return false;
                }

                console.log("XML convertido:", result);

                this.modelFacturaCFDI = this.mapearFactura(result) as FacturaCFDI;

                console.log("XML mapeado", this.modelFacturaCFDI);
            });
        };
        this.isLoading = false;
        reader.readAsText(file);
    }

    validaFacturaCFDI(facturaCFDI: FacturaCFDI): boolean {
        if (facturaCFDI.folio == null) {
            Swal.fire({
                title: 'Error',
                text: 'No se encontro el numero de folio',
                icon: 'error',
                showConfirmButton: false,
            });
            return false;
        }

        if (facturaCFDI.timbreFiscal.uuid == null) {
            Swal.fire({
                title: 'Error',
                text: 'No se encontro el UUID de la factura CFDI',
                icon: 'error',
                showConfirmButton: false,
            });
            return false;
        }

        if (facturaCFDI.fecha == null) {
            Swal.fire({
                title: 'Error',
                text: 'No se encontro la fecha de registro de la factura',
                icon: 'error',
                showConfirmButton: false,
            });
            return false;
        }

        if (facturaCFDI.impuestos.traslados[0] == null) {
            Swal.fire({
                title: 'Error',
                text: 'No se encontro el importe de traslado',
                icon: 'error',
                showConfirmButton: false,
            });
            return false;
        }

        return true;
    }

    validaDocumentos(): boolean {
        if (this.selectedPdf == null) {
            Swal.fire({
                title: 'Advertencia',
                text: 'Debe subir el archivo PDF de la factura CFDI',
                icon: 'warning',
                showConfirmButton: false,
            });
            return false;
        }

        if (this.selectedXml == null) {
            Swal.fire({
                title: 'Advertencia',
                text: 'Debe subir el archivo XML de la factura CFDI',
                icon: 'warning',
                showConfirmButton: false,
            });
            return false;
        }

        if (!this.selectedPdf.name.toLowerCase().endsWith(".pdf")) {
            Swal.fire({
                title: 'Advertencia',
                text: 'El archivo PDF de la factura CFDI no tiene la extencion correcta.',
                icon: 'warning',
                showConfirmButton: false,
            });
            return false;
        }

        if (!this.selectedXml.name.toLowerCase().endsWith(".xml")) {
            Swal.fire({
                title: 'Advertencia',
                text: 'El archivo XML de la factura CFDI no tiene la extencion correcta.',
                icon: 'warning',
                showConfirmButton: false,
            });
            return false;
        }

        if (!this.validaFacturaCFDI(this.modelFacturaCFDI)) {
            return false;
        }

        return true;
    }

    // ******** Funciones para limpiar modelos
    limpiaModelo() {
        this.selectedPdf = null;
        this.selectedXml = null;

        this.modelResultado = {
            estatus: false,
            mensaje: "",
            mensajeError: "",
            objeto: "",
            objetos: []
        };

        this.modelOrdenCompra = {
            idOrden: 0,
            tipo: "",
            estatus: "",
            fechaAlta: "",
            empresa: "",
            proveedor: "",
            idCliente: 0,
            cliente: "",
            elabora: "",
            observacion: "",
            inventario: 0,
            facturado: 0,
            idCredito: 0,
            credito: "",
            diasCredito: 0,
            idAlmacen: 0,
            loading: false,
            iva: 0,
            subTotal: 0,
            total: 0
        }

        this.modelFacturaCFDI = {
            version: "",
            fecha: "",
            serie: "",
            folio: "",
            subtotal: "",
            descuento: "",
            total: "",
            formaPago: "",
            metodoPago: "",
            lugarExpedicion: "",

            emisor: {
                rfc: "",
                nombre: "",
                regimenFiscal: ""
            },

            receptor: {
                rfc: "",
                nombre: "",
                usoCFDI: "",
                regimenFiscalReceptor: "",
                domicilioFiscal: ""
            },

            conceptos: [],

            impuestos: {
                totalTrasladados: "",
                totalRetenidos: "",
                traslados: []
            },

            timbreFiscal: {
                uuid: "",
                fechaTimbrado: "",
                rfcProvCertif: "",
                selloCFD: "",
                version: "",
                noCertificadoSAT: "",
                selloSAT: ""
            }
        };

        this.modelResultado = {
            estatus: false,
            mensaje: "",
            mensajeError: "",
            objeto: "",
            objetos: []
        }
    }


    limpiarPDF() {
        this.selectedPdf = null;
        this.pdfInput.nativeElement.value = '';
    }
    limpiarXML() {
        this.selectedXml = null;
        this.xmlInput.nativeElement.value = '';
        this.modelFacturaCFDI = {
            version: "",
            fecha: "",
            serie: "",
            folio: "",
            subtotal: "",
            descuento: "",
            total: "",
            formaPago: "",
            metodoPago: "",
            lugarExpedicion: "",

            emisor: {
                rfc: "",
                nombre: "",
                regimenFiscal: ""
            },

            receptor: {
                rfc: "",
                nombre: "",
                usoCFDI: "",
                regimenFiscalReceptor: "",
                domicilioFiscal: ""
            },

            conceptos: [],

            impuestos: {
                totalTrasladados: "",
                totalRetenidos: "",
                traslados: []
            },

            timbreFiscal: {
                uuid: "",
                fechaTimbrado: "",
                rfcProvCertif: "",
                selloCFD: "",
                version: "",
                noCertificadoSAT: "",
                selloSAT: ""
            }
        };
    }

    // ******** Funciones para guardar
    guardarFactura(): void {
        if (this.validaDocumentos()) {
            let xml: string;
            let xml1: string;
            let fFac: string = this.modelFacturaCFDI.fecha.split("T")[0];
            let fecFac: string[] = fFac.split("-");
            let fechaFactura: string = fecFac[0] + fecFac[1] + fecFac[2];

            if (this.ingresaInventario == 0) {
                // Com_pro_recepcione.aspx

                xml = `
            <Movimiento>
                <salida
                    factura="${this.modelFacturaCFDI.folio}"
                    uuid="${this.modelFacturaCFDI.timbreFiscal.uuid}"
                    fecfac="${fechaFactura}"
                    usuario="0"
                    idproveedor="${this.user.idProveedor}"
                    dias="${this.modelOrdenCompra.diasCredito}"
                    sub="${this.modelOrdenCompra.subTotal}"
                    iva="${this.modelOrdenCompra.iva}"
                    total="${this.modelOrdenCompra.total}"
                    rfc="${this.modelFacturaCFDI.receptor.rfc}"
                />
                <archivo
                    nombre=".pdf"
                />
                <archivo
                    nombre=".xml"
                />
            </Movimiento>`;

                xml1 = `
            <Movimiento>
                <comprobante
                    solicitud=""
                    fecha="${this.modelFacturaCFDI.fecha}"
                    factura="${this.modelFacturaCFDI.folio}"
                    importe="${this.modelFacturaCFDI.total}"
                />
                <archivo
                    nombre=".pdf"
                />
                <archivo
                    nombre=".xml"
                />
            </Movimiento>
            `;
                console.log(xml);
                console.log(xml1);

            } else {
                Swal.fire({
                    title: 'warning',
                    text: 'La factura CFDI no se puede subirt porque el comprador',
                    icon: 'error',
                    showConfirmButton: false,
                });
            }
        }
    }
}