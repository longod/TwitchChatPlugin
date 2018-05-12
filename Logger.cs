using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatPlugin {
    interface ILogger {
        void Error( string message );
        void Log( string message );
        void Debug( string message );
    }

    class ConsoleLogger : ILogger {
        public void Debug( string message ) {
#if DEBUG
            Console.WriteLine( "DEBUG: " + message );
#endif
        }

        public void Error( string message ) {
            Console.WriteLine( "ERROR: " + message );
        }

        public void Log( string message ) {
            Console.WriteLine( "LOG: " + message );
        }
    }

    class YukarinetteLogger : ILogger {
        public void Debug( string message ) {
#if DEBUG
            Yukarinette.YukarinetteLogger.Instance.Debug( message );
#endif
        }

        public void Error( string message ) {
            Yukarinette.YukarinetteLogger.Instance.Error( message );
        }

        public void Log( string message ) {
            Yukarinette.YukarinetteLogger.Instance.Info( message );
        }
    }
}
