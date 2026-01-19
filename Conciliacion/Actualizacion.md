# ============================================================================
# 🏦 PASO 1: SISTEMA DE RECONOCIMIENTO AUTOMÁTICO DE BANCOS
# ============================================================================

import os
import glob

# Obtener la carpeta donde está el script
CARPETA_TRABAJO = os.path.dirname(os.path.abspath(__file__))

# Lista de bancos dominicanos soportados
BANCOS_SOPORTADOS = {
    'POPULAR': ['POPULAR', 'BANCO POPULAR', 'BPD', 'BP'],
    'BANRESERVAS': ['BANRESERVAS', 'BANCO DE RESERVAS', 'RESERVAS', 'BDR'],
    'BHD': ['BHD', 'BANCO BHD', 'BHD LEON', 'BHDLEON'],
    'BANESCO': ['BANESCO', 'BANCO BANESCO'],
    'SCOTIABANK': ['SCOTIABANK', 'SCOTIA', 'BANCO SCOTIA'],
    'PROMERICA': ['PROMERICA', 'BANCO PROMERICA'],
    'SANTA_CRUZ': ['SANTA CRUZ', 'SANTACRUZ', 'BANCO SANTA CRUZ', 'BSC'],
    'ADEMI': ['ADEMI', 'BANCO ADEMI'],
    'LOPEZ_DE_HARO': ['LOPEZ DE HARO', 'LOPEZDEHARO', 'BANCO LOPEZ'],
    'BELLBANK': ['BELLBANK', 'BELL BANK']
}

def normalizar_nombre_banco(texto):
    """Normaliza el nombre para búsqueda (sin acentos, mayúsculas, sin espacios ni símbolos)"""
    if not texto:
        return ""
    texto = quitar_acentos(str(texto)).upper()
    texto = re.sub(r'[^A-Z0-9]', '', texto)  # Solo letras y números
    return texto

def detectar_banco_en_nombre_archivo(nombre_archivo):
    """
    Detecta qué banco es basándose en el nombre del archivo
    Retorna: (codigo_banco, nombre_legible) o (None, None)
    """
    nombre_norm = normalizar_nombre_banco(nombre_archivo)
    
    for codigo_banco, variantes in BANCOS_SOPORTADOS.items():
        for variante in variantes:
            variante_norm = normalizar_nombre_banco(variante)
            if variante_norm in nombre_norm:
                return codigo_banco, variante
    
    return None, None

def buscar_archivos_en_carpeta():
    """
    Busca automáticamente los archivos de banco y contable en la carpeta del script
    Retorna: (ruta_banco, ruta_contable, codigo_banco, nombre_banco_detectado)
    """
    print("\n" + "="*70)
    print("🔍 PASO 1: BUSCANDO ARCHIVOS EN LA CARPETA")
    print("="*70)
    print(f"📁 Carpeta de trabajo: {CARPETA_TRABAJO}\n")
    
    # Buscar todos los archivos Excel
    archivos_excel = []
    for extension in ['*.xlsx', '*.xls', '*.xlsm']:
        archivos_excel.extend(glob.glob(os.path.join(CARPETA_TRABAJO, extension)))
    
    if not archivos_excel:
        print("❌ No se encontraron archivos Excel (.xlsx, .xls) en la carpeta")
        return None, None, None, None
    
    print(f"📋 Archivos Excel encontrados: {len(archivos_excel)}")
    for archivo in archivos_excel:
        print(f"   • {os.path.basename(archivo)}")
    
    archivo_banco = None
    archivo_contable = None
    codigo_banco = None
    nombre_banco = None
    
    # Analizar cada archivo
    for ruta_archivo in archivos_excel:
        nombre_archivo = os.path.basename(ruta_archivo)
        nombre_sin_extension = os.path.splitext(nombre_archivo)[0]
        
        # Verificar si es el archivo CONTABLE
        nombre_norm = normalizar_nombre_banco(nombre_sin_extension)
        if nombre_norm in ['CONTABLE', 'LIBROCONTABLE', 'LIBRO', 'ACCOUNTINGBOOK']:
            archivo_contable = ruta_archivo
            print(f"\n✅ CONTABLE identificado: {nombre_archivo}")
            continue
        
        # Verificar si es un archivo de BANCO
        banco_det, nombre_det = detectar_banco_en_nombre_archivo(nombre_sin_extension)
        if banco_det:
            archivo_banco = ruta_archivo
            codigo_banco = banco_det
            nombre_banco = nombre_det
            print(f"\n✅ BANCO identificado: {nombre_archivo}")
            print(f"   🏦 Banco: {codigo_banco} ({nombre_det})")
    
    print("\n" + "─"*70)
    
    # Validar resultados
    if not archivo_banco:
        print("\n❌ ERROR: No se encontró archivo de banco")
        print("   El nombre del archivo debe contener el nombre del banco")
        print(f"   Bancos soportados: {', '.join(BANCOS_SOPORTADOS.keys())}")
        return None, None, None, None
    
    if not archivo_contable:
        print("\n❌ ERROR: No se encontró archivo CONTABLE")
        print("   El archivo debe llamarse 'CONTABLE.xlsx' o similar")
        return None, None, None, None
    
    print("\n✅ Archivos validados correctamente")
    print(f"   🏦 Banco:    {os.path.basename(archivo_banco)}")
    print(f"   📗 Contable: {os.path.basename(archivo_contable)}")
    
    return archivo_banco, archivo_contable, codigo_banco, nombre_banco

# ============================================================================
# PASO 2: SISTEMA DE LIMPIEZA ESPECÍFICO POR BANCO
# ============================================================================

def limpiar_banco_popular(df_original):
    """
    Limpieza específica para BANCO POPULAR
    
    INSTRUCCIONES PARA CONFIGURAR:
    1. Abre un archivo real de Banco Popular
    2. Identifica:
       - ¿En qué fila empiezan los datos reales? (después de encabezados)
       - ¿Cómo se llaman las columnas de Fecha, Concepto/Descripción?
       - ¿Hay columnas separadas de Débito y Crédito? ¿O una sola de Valor?
       - ¿Hay filas de totales al final que debemos ignorar?
    3. Configura esta función según lo que observes
    """
    print("\n   Aplicando limpieza: BANCO POPULAR")
    print("     ESTA FUNCIÓN NECESITA SER CONFIGURADA CON DATOS REALES")
    
    df = df_original.copy()
    
    # EJEMPLO - AJUSTAR SEGÚN FORMATO REAL:
    # ---------------------------------------
    
    # 1. Eliminar filas de encabezado (si hay varias filas antes de los datos)
    # df = df.iloc[5:]  # Por ejemplo, si los datos empiezan en la fila 6
    
    # 2. Resetear índice
    df = df.reset_index(drop=True)
    
    # 3. Eliminar filas completamente vacías
    df = df.dropna(how='all')
    
    # 4. Identificar y renombrar columnas
    # OPCIÓN A: Si tiene Débito y Crédito separados
    """
    df = df.rename(columns={
        'FECHA': 'Fecha',
        'DESCRIPCION': 'Concepto',
        'DEBITO': 'Debito',
        'CREDITO': 'Credito',
        'REFERENCIA': 'Descripción'
    })

# Mapeo de bancos a funciones de limpieza
FUNCIONES_LIMPIEZA_BANCO = {
    'POPULAR': limpiar_banco_popular,
    'BANRESERVAS': limpiar_banreservas,
    'BHD': limpiar_bhd,
    'BANESCO': limpiar_banesco,
    'SCOTIABANK': limpiar_scotiabank,
    'PROMERICA': limpiar_promerica,
    'SANTA_CRUZ': limpiar_santa_cruz,
    'ADEMI': limpiar_ademi,
    'LOPEZ_DE_HARO': limpiar_lopez_de_haro,
    'BELLBANK': limpiar_bellbank
}

def limpiar_archivo_banco(ruta_archivo, codigo_banco):
    """
    Carga y limpia el archivo del banco según su código
    """
    print(f"\n  📂 Cargando archivo de {codigo_banco}...")
    
    try:
        # Leer archivo Excel completo (sin procesar)
        df_crudo = pd.read_excel(ruta_archivo)
        print(f"  📊 Dimensiones originales: {df_crudo.shape[0]} filas × {df_crudo.shape[1]} columnas")
        print(f"  📋 Columnas originales: {list(df_crudo.columns)}")
        
        # Aplicar limpieza específica del banco
        if codigo_banco in FUNCIONES_LIMPIEZA_BANCO:
            df_limpio = FUNCIONES_LIMPIEZA_BANCO[codigo_banco](df_crudo)
        else:
            print(f"  ⚠️  No hay función de limpieza para {codigo_banco}")
            df_limpio = df_crudo
        
        # Validar que tengamos las 4 columnas necesarias
        columnas_requeridas = ['Fecha', 'Concepto', 'Valor', 'Descripción']
        columnas_presentes = [col for col in columnas_requeridas if col in df_limpio.columns]
        
        if len(columnas_presentes) != 4:
            print(f"\n  ❌ ERROR: El archivo limpio no tiene las 4 columnas necesarias")
            print(f"     Requeridas: {columnas_requeridas}")
            print(f"     Presentes:  {columnas_presentes}")
            print(f"     Columnas actuales: {list(df_limpio.columns)}")
            print(f"\n  💡 Debes configurar la función de limpieza para {codigo_banco}")
            return None
        
        # Seleccionar solo las 4 columnas en el orden correcto
        df_final = df_limpio[columnas_requeridas].copy()
        
        print(f"  ✅ Limpieza completada: {df_final.shape[0]} filas × {df_final.shape[1]} columnas")
        return df_final
        
    except Exception as e:
        print(f"  ❌ Error al limpiar archivo: {e}")
        return None

# ============================================================================
# PASO 2B: LIMPIEZA DEL ARCHIVO CONTABLE (UNIVERSAL)
# ============================================================================

def limpiar_archivo_contable(ruta_archivo, moneda='RD$'):
    """
    Limpia el archivo contable (formato estándar para todos los casos)
    
    PARÁMETROS:
    - ruta_archivo: ruta del archivo Excel
    - moneda: 'RD$' (pesos) o 'USD' (dólares) - para futura implementación

    ---CODIGO PARA LIMPIEZA---
    
    """
    print(f"\n  📂 Cargando archivo CONTABLE ({moneda})...")

# ============================================================================
# PASO 3: CARGAR Y PROCESAR DATOS AUTOMATICAMENTE
# ============================================================================