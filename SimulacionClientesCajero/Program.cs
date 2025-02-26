Random random = new Random();

int numCajas = 3;
int numClientes = 20;
double tasaLlegada = 1.0 / 5.0;
double tiempoAtencion = 4.0;

List<double> tiemposEspera = Simular(numCajas, numClientes, tasaLlegada, tiempoAtencion);

Console.WriteLine("Realizando prueba KS...");
double pValor = PruebaKS(tiemposEspera, tasaLlegada);
Console.WriteLine($"P-Valor: {pValor}");

if (pValor < 0.05)
{
    Console.WriteLine("La simulación no sigue la distribución esperada.");
}
else
{
    Console.WriteLine("No hay evidencia suficiente para rechazar la hipótesis de que los datos siguen la distribución esperada.");
}

List<double> Simular(int numCajas, int numClientes, double tasaLlegada, double tiempoAtencion)
{
    Queue<double>[] filas = new Queue<double>[numCajas];
    for (int i = 0; i < numCajas; i++) filas[i] = new Queue<double>();

    List<double> tiemposEspera = new List<double>();
    double tiempoActual = 0;

    for (int i = 0; i < numClientes; i++)
    {
        double llegada = tiempoActual + (-Math.Log(1 - random.NextDouble()) / tasaLlegada);
        int cajaSeleccionada = SeleccionarCaja(filas);
        double inicioAtencion = Math.Max(llegada, filas[cajaSeleccionada].Count > 0 ? filas[cajaSeleccionada].Peek() : 0);
        double finAtencion = inicioAtencion + tiempoAtencion;
        tiemposEspera.Add(inicioAtencion - llegada);
        filas[cajaSeleccionada].Enqueue(finAtencion);
        tiempoActual = llegada;

        Console.WriteLine($"Cliente {i + 1}: Llega en t={llegada:F2}, Caja {cajaSeleccionada + 1}, " +
                  $"Inicio Atención={inicioAtencion:F2}, Fin Atención={finAtencion:F2}, " +
                  $"Tiempo de espera={inicioAtencion - llegada:F2}");

    }

    return tiemposEspera;
}

int SeleccionarCaja(Queue<double>[] filas)
{
    int mejorCaja = 0;
    double menorTiempo = double.MaxValue;
    for (int i = 0; i < filas.Length; i++)
    {
        double tiempoLibre = filas[i].Count > 0 ? filas[i].Peek() : 0;
        if (tiempoLibre < menorTiempo)
        {
            menorTiempo = tiempoLibre;
            mejorCaja = i;
        }
    }
    return mejorCaja;
}

double PruebaKS(List<double> datos, double tasa)
{
    datos.Sort();
    int n = datos.Count;
    double dMax = 0.0;

    for (int i = 0; i < n; i++)
    {
        double empCDF = (i + 1) / (double)n;
        double teorCDF = 1 - Math.Exp(-tasa * datos[i]);
        double diferencia = Math.Abs(empCDF - teorCDF);
        if (diferencia > dMax)
        {
            dMax = diferencia;
        }
    }

    double pValor = Math.Exp(-2 * n * dMax * dMax);
    return pValor;
}
