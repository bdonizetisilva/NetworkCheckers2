using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NetworkCheckers.Utils
{
    /// <summary>
    /// Métodos auxiliares de <i>Thread</i>
    /// </summary>
    public static class ThreadHelper
    {
        /// <summary>
        /// Tenta executar o comando de <i>Abort</i> sem disparar exceções
        /// </summary>
        /// <param name="source">Thread a ser abortada</param>
        /// <returns><i>null</i> caso abortar sem problemas</returns>
        public static Thread SafeAbort(this Thread source)
        {
            // Possui elemento?
            if (source != null)
            {
                try
                {
                    // Aborta a thread
                    source.Abort();

                    // Retorna nada, pois a thread foi interrompida
                    return null;
                }

                // Repassa thread abortada
                catch (ThreadAbortException) { throw; }

                // Ignora outras exceções
                catch {; }
            }

            // Retorna a fonte
            return source;
        }

        /// <summary>
        /// Tenta executar o comando de <i>Abort</i> sem disparar exceções
        /// </summary>
        /// <param name="source">Thread a ser abortada</param>
        /// <param name="timeout">Tempo de vida antes de destruir a thread</param>
        public static void SafeAbort(this Thread source, int timeout)
        {
            // Possui elemento?
            if (source != null)
            {
                try
                {
                    // Tempo limite
                    int ttl = Environment.TickCount + timeout;

                    // Thread ativa?
                    while (source.IsAlive)
                    {
                        // Tempo limite estourado? Sai do laço
                        if (Environment.TickCount > ttl)
                            break;

                        // Aguarda
                        Thread.Sleep(1);
                    }

                    // Aborta a thread
                    source.SafeAbort();
                }

                // Repassa thread abortada
                catch (ThreadAbortException) { throw; }

                // Outras exceções, aborta thread diretamente
                catch { source.SafeAbort(); }
            }
        }

        /// <summary>
        /// Aborta determinado conjunto de threads
        /// </summary>
        /// <param name="source">Threads a serem abortadas</param>
        /// <param name="timeout">Tempo de vida antes de destruir a thread</param>
        public static void SafeAbort(int timeout, params Thread[] source)
        {
            // Possui elementos?
            if ((source != null) && (source.Length > 0))
            {
                // Primeira parte é aguardar por threads
                try
                {
                    // Tempo limite
                    int ttl = Environment.TickCount + timeout;

                    // Loop
                    while (true)
                    {
                        // Tempo limite estourado? Sai do laço
                        if (Environment.TickCount > ttl)
                            break;

                        // Quantidade de threads ativas
                        int active = 0;

                        // Há alguma thread ativa?
                        for (int i = 0, c = source.Length; i < c; i++)
                        {
                            // Ativa?
                            Thread thread = source[i];
                            if ((thread != null) && (thread.IsAlive))
                                active++;
                        }

                        // Não há threads ativas?
                        if (active == 0)
                            return;

                        // Aguarda
                        Thread.Sleep(1);
                    }
                }

                // Repassa thread abortada
                catch (ThreadAbortException) { throw; }

                // Ignora outras exceções
                catch {; }

                // Segunda parte é abortar todas as threads
                try
                {
                    // Para cada thread
                    for (int i = 0, c = source.Length; i < c; i++)
                    {
                        try
                        {
                            // Aborta thread
                            source[i]?.Abort();
                        }

                        // Repassa thread abortada
                        catch (ThreadAbortException) { throw; }

                        // Ignora outras exceções
                        catch {; }
                    }
                }

                // Repassa thread abortada
                catch (ThreadAbortException) { throw; }

                // Ignora outras exceções
                catch {; }
            }
        }
    }
}
