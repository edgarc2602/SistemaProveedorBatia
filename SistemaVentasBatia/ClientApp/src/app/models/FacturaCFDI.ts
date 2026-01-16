export interface FacturaCFDI {
    version: string;
    fecha: string;
    sello?: string;
    noCertificado?: string;
    certificado?: string;
    condicionesDePago?: string;

    serie?: string;
    folio?: string;

    formaPago?: string;
    metodoPago?: string;
    tipoDeComprobante?: string;
    exportacion?: string;
    moneda?: string;

    subtotal: string;
    descuento?: string;
    total: string;
    lugarExpedicion: string;

    emisor: EmisorCFDI;
    receptor: ReceptorCFDI;

    conceptos: ConceptoCFDI[];

    impuestos?: ImpuestosCFDI;

    timbreFiscal?: TimbreFiscalCFDI | undefined;
}

/* ===== EMISOR ===== */
export interface EmisorCFDI {
    rfc: string;
    nombre: string;
    regimenFiscal: string;
}

/* ===== RECEPTOR ===== */
export interface ReceptorCFDI {
    rfc: string;
    nombre: string;
    domicilioFiscal?: string;
    regimenFiscalReceptor: string;
    usoCFDI: string;
}

/* ===== CONCEPTOS ===== */
export interface ConceptoCFDI {
    claveProdServ: string;
    noIdentificacion?: string;
    cantidad: string;
    claveUnidad: string;
    unidad?: string;
    descripcion: string;
    valorUnitario: string;
    importe: string;
    descuento?: string;
    objetoImp: string;

    impuestos?: ImpuestosConceptoCFDI;

    cuentaPredial?: CuentaPredialCFDI;
}

export interface ImpuestosConceptoCFDI {
    traslados?: TrasladoCFDI[];
    retenciones?: RetencionConceptoCFDI[];
}

export interface CuentaPredialCFDI {
    numero: string;
}

/* ===== IMPUESTOS GLOBALES ===== */
export interface ImpuestosCFDI {
    totalRetenidos?: string;
    totalTrasladados?: string;

    traslados?: TrasladoCFDI[];
    retenciones?: RetencionGlobalCFDI[];
}

/* ===== TRASLADOS ===== */
export interface TrasladoCFDI {
    base: string;
    impuesto: string;
    tipoFactor: string;
    tasaOCuota: string;
    importe: string;
}

/* ===== RETENCIONES DE CONCEPTO ===== */
export interface RetencionConceptoCFDI {
    base: string;
    impuesto: string;
    tipoFactor: string;
    tasaOCuota: string;
    importe: string;
}

/* ===== RETENCIONES GLOBALES ===== */
export interface RetencionGlobalCFDI {
    impuesto: string;
    importe: string;
}

/* ===== TIMBRE FISCAL ===== */
export interface TimbreFiscalCFDI {
    version?: string;
    uuid?: string;
    fechaTimbrado?: string;
    rfcProvCertif?: string;
    selloCFD?: string;
    noCertificadoSAT?: string;
    selloSAT?: string;
}
