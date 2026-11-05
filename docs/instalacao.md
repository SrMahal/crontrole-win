# Instalação e execução

[← Índice da documentação](README.md)

## Requisitos

- **Windows 11** para a interface desktop WPF.
- Controle de videogame compatível com **XInput**, conectado e reconhecido pelo Windows.
- **.NET 10 SDK** para compilar ou executar a partir do código-fonte. O ZIP pronto para uso requer o **.NET 10 Desktop Runtime**; o SDK também o fornece.
- Não é necessário substituir drivers do controle nem usar privilégios administrativos para a instalação padrão.

## Confirmar o controle

Pressione `Win + R`, digite `joy.cpl` e verifique se o dispositivo aparece e responde na aba de teste. A interface pode aparecer como controle compatível com Xbox 360. O nome de exibição não identifica necessariamente o fabricante físico.

## Clonar o código e executar

No **PowerShell do Windows**, e não no WSL:

```powershell
git clone https://github.com/SrMahal/crontrole-win.git
cd crontrole-win
& 'C:\Program Files\dotnet\dotnet.exe' --version
& 'C:\Program Files\dotnet\dotnet.exe' run --project .\src\Kp4040.App\Kp4040.App.csproj
```

Se o comando `dotnet` já estiver no PATH, use simplesmente `dotnet run --project .\src\Kp4040.App\Kp4040.App.csproj`.

Também é possível usar a opção **Code → Download ZIP** no GitHub. Nesse caso, extraia o arquivo e abra o PowerShell na pasta que contém `Kp4040.App.slnx`.

O executável publicado se chama `Crontrole.exe`; o projeto C# ainda conserva o nome interno `Kp4040.App`.

## Executar o pacote pronto

Baixe o [ZIP Windows x64](../dist/Crontrole-Alpha-v1.0-win-x64.zip), extraia todos os quatro arquivos na mesma pasta e abra `Crontrole.exe`. O ZIP depende do .NET 10 Desktop Runtime instalado no Windows.

## Primeira utilização

1. Conecte o controle e abra a aplicação.
2. Verifique o status de conexão e os valores dos analógicos.
3. As opções **Habilitar execução de ações** e **Analógicos: mouse e scroll** começam marcadas. Desmarque-as se desejar configurar sem enviar entradas.
4. Use **Identificar botão** para localizar uma entrada e definir seu comportamento.
5. Para testar atalhos de teclado, deixe as ações habilitadas e coloque o foco em outro aplicativo, como o Bloco de Notas.
6. Para cursor/rolagem, mantenha também **Analógicos: mouse e scroll** marcada.

Veja os detalhes em [Guia de uso](uso.md).

## Problemas comuns

| Sintoma | O que verificar |
| --- | --- |
| `dotnet` não reconhecido | Instalação do SDK e PATH. Você pode chamar `C:\Program Files\dotnet\dotnet.exe` diretamente. |
| Caminho do `.csproj` não existe | Execute o comando na raiz do repositório. |
| Controle não aparece | Conexão USB, `joy.cpl` e compatibilidade XInput. |
| O mouse não se move | Marque **ambas** as opções: execução de ações e analógicos. |
| Movimento involuntário do mouse | Centre os sticks e verifique deriva ou calibração; há uma zona morta fixa no código atual. |
| Atalho não responde no aplicativo elevado | `SendInput` pode ser limitado pela integridade/privilégios da janela de destino. |

Para reportar um problema: [abrir uma issue](https://github.com/SrMahal/crontrole-win/issues).
