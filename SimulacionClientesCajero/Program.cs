Random random = new Random();

int numCajas = 3;
int numClientes = 20;
double tasaLlegada = 1.0 / 2.5;
double tiempoAtencion = 4.0;

List<double> tiemposEspera = Simular(numCajas, numClientes, tasaLlegada, tiempoAtencion);

Console.WriteLine("\nRealizando prueba KS...");
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
    double[] tiempoDisponible = new double[numCajas];

    List<double> tiemposEspera = new List<double>();
    double tiempoActual = 0;

    for (int i = 0; i < numClientes; i++)
    {
        double llegada = tiempoActual + (-Math.Log(1 - random.NextDouble()) / tasaLlegada);

        int cajaSeleccionada = SeleccionarCaja(tiempoDisponible);

        double inicioAtencion = Math.Max(llegada, tiempoDisponible[cajaSeleccionada]);
        double finAtencion = inicioAtencion + tiempoAtencion;
        double tiempoEspera = inicioAtencion - llegada;

        tiemposEspera.Add(tiempoEspera);
        tiempoDisponible[cajaSeleccionada] = finAtencion;

        if (i == 0)
        {
            Console.WriteLine("\n╔══════════╦═══════════════╦══════════╦═════════════════╦════════════════╦═══════════════╗");
            Console.WriteLine("║ Cliente  ║  Llega (seg)  ║  Caja    ║  Inicio  (seg)  ║  Fin     (seg) ║  Espera (seg) ║");
            Console.WriteLine("╠══════════╬═══════════════╬══════════╬═════════════════╬════════════════╬═══════════════╣");
        }

        Console.WriteLine($"║ {i + 1,-8} ║ {llegada,-8:F2}      ║ {cajaSeleccionada + 1,-8} ║ {inicioAtencion,-8:F2}        ║ {finAtencion,-12:F2}   ║ {tiempoEspera,-8:F2}      ║");

        if (i == numClientes - 1)
        {
            Console.WriteLine("╚══════════╩═══════════════╩══════════╩═════════════════╩════════════════╩═══════════════╝");
        }

        tiempoActual = llegada;
    }

    return tiemposEspera;
}

int SeleccionarCaja(double[] tiempoDisponible)
{
    int mejorCaja = 0;
    double menorTiempo = tiempoDisponible[0];

    for (int i = 1; i < tiempoDisponible.Length; i++)
    {
        if (tiempoDisponible[i] < menorTiempo)
        {
            menorTiempo = tiempoDisponible[i];
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