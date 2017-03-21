namespace NetworkCheckers.UI
{
    /// <summary>
    /// Representa um timer.
    /// </summary>
    internal class Timer
    {
        // Membros estáticos
        #region Estaticos
        
        /// <summary>
        /// Guarda se o timer já foi inicializado
        /// </summary>
        private static bool _isTimerInitialized;

        /// <summary>
        /// Guarda se o timer utiliza frequencia
        /// </summary>
        private static bool _mBUsingQpf;

        /// <summary>
        /// Guarda quantos ticks por segundos
        /// </summary>
        private static long _mLlQpfTicksPerSec;

        /// <summary>
        /// Ticks base para calculo usando frequencia
        /// </summary>
        private static long _mLlBaseTime;

        /// <summary>
        /// Base para calculo usando hora
        /// </summary>
        private static double _mFBaseTime;

        #endregion

        #region Public

        /// <summary>
        /// Retorna a hora atual do pc em segundos.
        /// Deve ser usado relativamente, uma vez que o valor da fonte pode mudar.
        /// </summary>
        /// <returns></returns>
        public static float Query()
        {
            // Se o timer não está inicializado
            if (!_isTimerInitialized)
            {
                // Marca como inicializado
                _isTimerInitialized = true;

                long qwTicksPerSec = 0;

                // Usa QueryPerformanceFrequency() para recuperar se o timer usa a frequencia.
                _mBUsingQpf = QueryPerformanceFrequency(ref qwTicksPerSec);

                // Se usa frequencia
                if (_mBUsingQpf)
                {
                    // Recupera o timer com base na frequencia
                    _mLlQpfTicksPerSec = qwTicksPerSec;
                    QueryPerformanceCounter(ref _mLlBaseTime);
                }
                // Se não usa frequencia
                else
                {
                    // Recupera a hora para usar como timer
                    _mFBaseTime = (float)(timeGetTime() * 0.001);
                }

                // Retorna INIT
                return 0;
            }

            // Se usa frequencia
            if (_mBUsingQpf)
            {
                long qwTime = 0;

                // Recupera tempo atual usando a frequencia
                QueryPerformanceCounter(ref qwTime);

                // Calcula diferenca do timer e retorna
                return (float)((double)(qwTime - _mLlBaseTime) / (double)(_mLlQpfTicksPerSec));
            }
            // Se não usa frequencia
            else
            {
                // Recupera a hora
                double dTime = (timeGetTime() * 0.001);

                // Calcula a diferenca do timer e retorna
                return (float)(dTime - _mFBaseTime);
            }
        }

        #endregion

        // Chamadas externas do windows
        #region Win32 Members

        [System.Security.SuppressUnmanagedCodeSecurity]
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern bool QueryPerformanceFrequency(ref long performanceFrequency);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern bool QueryPerformanceCounter(ref long performanceCount);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        static extern int timeGetTime();

        #endregion
    }
}
