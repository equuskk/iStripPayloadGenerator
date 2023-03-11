using System.Security.Cryptography;

namespace IStripLight;

public class PayloadGenerator
{
    /// <summary>
    /// Ключ шифрования данных
    /// </summary>
    private static readonly byte[] Key =
    {
        0x34,
        0x52,
        0x2A,
        0x5B,
        0x7A,
        0x6E,
        0x49,
        0x2C,
        0x08,
        0x09,
        0x0A,
        0x9D,
        0x8D,
        0x2A,
        0x23,
        0xF8
    };

    /// <summary>
    /// Шапка для запроса - всегда статичная
    /// </summary>
    private static readonly byte[] Header =
    {
        0x54,
        0x52,
        0x0,
        0x57
    };

    private readonly ICryptoTransform _crypt;
    private const int GroupId = 1; // должен быть больше нуля

    public PayloadGenerator()
    {
        var aes = Aes.Create();
        aes.Mode = CipherMode.ECB; // либо так, либо вместо нулл пустой массив из нулей
        _crypt = aes.CreateEncryptor(Key, null);
    }

    /// <summary>
    /// Получить payload для установки конкретного цвета лампы
    /// </summary>
    /// <param name="red">Красный спектр</param>
    /// <param name="green">Зелёный спектр</param>
    /// <param name="blue">Синий спектр</param>
    /// <param name="brightness">Яркость лампы (от 0 до 100)</param>
    /// <param name="speed">Скорость смены эффектов (от 0 до 100)</param>
    /// <returns>payload для установки конкретного цвета лампы</returns>
    public string GetRgbPayload(byte red, byte green, byte blue, byte brightness = 100, byte speed = 100)
    {
        var payload = new byte[16]
        {
            Header[0],
            Header[1],
            Header[2],
            Header[3],

            (byte)CommandType.Rgb,
            GroupId,

            0, // mode?

            red,
            green,
            blue,

            brightness,
            speed,

            0x0,
            0x0,
            0x0,
            0x0
        };

        var result = new byte[16];
        _crypt.TransformBlock(payload, 0, payload.Length, result, 0);

        return ConvertToHexString(result);
    }

    private static string ConvertToHexString(IEnumerable<byte> payload)
    {
        return string.Join("", payload.Select(x => x.ToString("X2").ToLower()));
    }
}

public enum CommandType : byte
{
    /// <summary>
    /// Запрос на вступление в группу
    /// </summary>
    JoinGroupRequest = 1,

    /// <summary>
    /// Установить конкретный цвет лампы
    /// </summary>
    Rgb = 2,

    /// <summary>
    /// Установить режим свечения в такт музыки
    /// </summary>
    Rhythm = 3,

    /// <summary>
    /// Установить таймер работы лампы
    /// </summary>
    Timer = 4,

    /// <summary>
    /// 
    /// </summary>
    RgbLineSequence = 5,

    /// <summary>
    /// Установить скорость работы эффекта
    /// </summary>
    Speed = 6,

    /// <summary>
    /// Установить яркость лампы
    /// </summary>
    Light = 7
}