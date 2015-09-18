using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Novensys.eCard.SDK;

namespace Cnas.eCard.SDK
{
    public static class TerminalManagerFixer
    {
        public static void FixUmOfflineStates( ISesiuneCard sesiuneCard )
        {
            // check if valid instance
            if( sesiuneCard == null )
                return;

            // get the target method
            Assembly sdk = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault( a => a.GetName().Name == "Novensys.eCard.SDK" );
            if( sdk == null )
                return;

            // get internal property info
            PropertyInfo terminalManagerProp = sesiuneCard.GetType().GetProperty( "TerminalManager", BindingFlags.NonPublic | BindingFlags.Instance );
            if( terminalManagerProp == null )
                return;

            // get internal property value
            object terminalManager = terminalManagerProp.GetValue( sesiuneCard, null );
            if( terminalManager == null )
                return;

            // get internal property info
            PropertyInfo offlineErrorsProp = terminalManager.GetType().GetProperty( "RaspunsuriUMPentruOffline", BindingFlags.NonPublic | BindingFlags.Instance );
            if( terminalManagerProp == null )
                return;

            // get internal property value (cast to known type)
            var offlineErrors = offlineErrorsProp.GetValue( terminalManager, null ) as List<CoduriRaspunsOperatieCard>;
            if( offlineErrors == null )
                return;

            // apply fixup
            if( offlineErrors.Contains( CoduriRaspunsOperatieCard.ERR_UM_STARE_CARD_INVALIDA ) == false )
            {
                offlineErrors.Add( CoduriRaspunsOperatieCard.ERR_UM_STARE_CARD_INVALIDA );
            }
        }
    }
}
