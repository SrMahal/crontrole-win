# Instalação e execução

[← Índice da documentação](README.md)

## Requisitos

- **Windows 11** para a interface desktop WPF.
- Controle de videogame compatível com **XInput**, conectado e reconhecido pelo Windows.
- **.NET 10 SDK** para compilar ou executar a partir do código-fonte. Um pacote de release *self-contained*, quando publicado, pode dispensar esse SDK para o usuário final.
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

> **Importante:** o executável e o projeto ainda têm o nome interno `Kp4040.App` na base publicada; *Crontrole* é a nova identidade pública planejada para o lançamento Alpha v1.0.

## Primeira utilização

1. Conecte o controle e abra a aplicação.
2. Verifique o status de conexão e os valores dos analógicos.
3. Mantenha **Habilitar execução de ações** desmarcado durante a configuração.
4. Use **Identificar botão** para localizar uma entrada e definir seu comportamento.
5. Para testar atalhos, habilite as ações e coloque o foco em outro aplicativo, como o Bloco de Notas.
6. Para cursor/rolagem, habilite também **Analógicos: mouse e scroll**.

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
