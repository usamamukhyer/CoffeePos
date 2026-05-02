using Microsoft.Maui.Networking;

namespace DBCafeteria.Services;

public static class NetworkErrorMessages
{
    public static string ApiUnavailable(string area) =>
        Connectivity.Current.NetworkAccess == NetworkAccess.Internet
            ? $"{area} no pudo conectarse al servidor. Revisa el servicio o el tunel de ngrok."
            : $"No tienes conexion. {area} usara los datos guardados cuando esten disponibles.";
}
