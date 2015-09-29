using System;
using System.Reflection;
using Novensys.eCard.SDK;
using Novensys.eCard.SDK.Entities.SmartCard;
using Novensys.eCard.SDK.Utils.Crypto;

namespace Cnas.eCard.SDK
{
    public static class SmartCardStatesFixer
    {
        public static bool FixKnownCardIssues( ISesiuneCard sesiuneCard, ref CoduriRaspunsOperatieCard response )
        {
            if( sesiuneCard != null && sesiuneCard.Token != null && // check if online sesion exists
                sesiuneCard.StareCard == StareCard.Activ && sesiuneCard.NumarIncercariRamase == 5 ) // check if pin was validated on card
            {
                var terminalManager = sesiuneCard.GetPropertyValue<object>( "TerminalManager" );
                if( terminalManager != null )
                {
                    var flagStareActivare = terminalManager.GetPropertyValue<ActivateStatus>( "FlagStareActivare" );
                    if( flagStareActivare == ActivateStatus.PIN_TRANSP_CHANGED_ON_CARD ) // check if transport pin not syncronized online
                    {
                        if( sesiuneCard.StareComunicatieCuUM == StareComunicatieCuUM.COMUNICATIE_OK &&
                            sesiuneCard.StareAutentificare == StariAutentificare.AUTENTIFICAT &&
                            sesiuneCard.StareCardInTerminal == StareCardInTerminal.CardInserat )
                        {
                            if( response == CoduriRaspunsOperatieCard.ERR_UM_STARE_CARD_INVALIDA )
                            {
                                terminalManager.InvokeMethod( "WriteActivateStatus", ActivateStatus.UM_PIN_SYNCRONIZED, ActivateStatusExtern.NECUNOSCUT );
                                response = CoduriRaspunsOperatieCard.OK;
                                return true;
                            }
                        }
                    }
                    else if( flagStareActivare == ActivateStatus.UM_PIN_SYNCRONIZED ) // check if card was activated online
                    {
                        if( sesiuneCard.StareAutentificare == StariAutentificare.AUTENTIFICARE_ESUATA &&
                            sesiuneCard.StareCardInTerminal == StareCardInTerminal.CardInserat )
                        {
                            var pinBlock = terminalManager.GetPropertyValue<string>( "PINBlock" );
                            if( pinBlock == null ) // check if pin was validated on card
                            {
                                return false;
                            }
                            switch( response )
                            {
                                case CoduriRaspunsOperatieCard.ERR_INVALID_CARD:
                                    // reset activation status flag
                                    terminalManager.InvokeMethod( "WriteActivateStatus", ActivateStatus.PIN_TRANSP_CHANGED_ON_CARD, ActivateStatusExtern.NECUNOSCUT );
                                    // trigger card activation and check putput
                                    if( (int)CoduriRaspunsOperatieCard.OK == sesiuneCard.ActiveazaCard( sesiuneCard.Token ) )
                                    {
                                        response = CoduriRaspunsOperatieCard.OK;
                                        return true;
                                    }
                                    break;
                                case CoduriRaspunsOperatieCard.ERR_INVALID_PIN:
                                case CoduriRaspunsOperatieCard.ERR_CARD_BLOCKED:
                                    var umClient = terminalManager.GetPropertyValue<object>( "UMClient" );
                                    var cardNumber = terminalManager.GetPropertyValue<string>( "CardNumber" );
                                    var terminalId = terminalManager.GetPropertyValue<string>( "TerminalId" );
                                    var terminalData = terminalManager.GetPropertyValue<object>( "TerminalData" );
                                    var certificateSerialNumber = terminalManager.GetPropertyValue<string>( "CertificateSerialNumber" );
                                    if( umClient != null && cardNumber != null && terminalId != null && terminalData != null && certificateSerialNumber != null ) // pin was validated on card
                                    {
                                        var cheieCriptarePIN = terminalData.GetPropertyValue<string>( "CheieCriptarePIN" );
                                        if( cheieCriptarePIN != null )
                                        {
                                            int retryCount = sesiuneCard.NumarIncercariRamase;
                                            string resetPinBlock = CryptoHelper.CreatePinBlock( cardNumber, "0000", cheieCriptarePIN );
                                            if( pinBlock != null && resetPinBlock != null )
                                            {
                                                if( CoduriRaspunsOperatieCard.OK == umClient.InvokeMethod<CoduriRaspunsOperatieCard>( "ExecutaResetarePIN", sesiuneCard.Token, cardNumber, terminalId, resetPinBlock, certificateSerialNumber ) &&
                                                    CoduriRaspunsOperatieCard.OK == umClient.InvokeMethod<CoduriRaspunsOperatieCard>( "ExecutaSchimbarePIN", sesiuneCard.Token, cardNumber, retryCount, terminalId, resetPinBlock, pinBlock, certificateSerialNumber ) )
                                                {
                                                    response = CoduriRaspunsOperatieCard.OK;
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return false;
        }

        #region Helpers
        public static T GetPropertyValue<T>( this object instance, string propName )
        {
            PropertyInfo pi = instance.GetType().GetProperty( propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            if( pi == null )
            {
                throw new ArgumentOutOfRangeException( "propName", string.Format( "Property {0} was not found in Type {1}", propName, instance.GetType().FullName ) );
            }
            return (T)pi.GetValue( instance, null );
        }

        public static void InvokeMethod( this object instance, string methodName, params object[] parameters )
        {
            InvokeMethod<object>( instance, methodName, parameters );
        }

        public static T InvokeMethod<T>( this object instance, string methodName, params object[] parameters )
        {
            Type t = instance.GetType();
            MethodInfo mi = null;
            while( mi == null && t != null )
            {
                mi = t.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
                t = t.BaseType;
            }
            if( mi == null )
            {
                throw new ArgumentOutOfRangeException( "methodName", string.Format( "Method {0} was not found in Type {1}", methodName, instance.GetType().FullName ) );
            }
            return (T)mi.Invoke( instance, parameters );
        }
        #endregion
    }
}
