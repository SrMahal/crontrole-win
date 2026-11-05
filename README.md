# 🎮 Crontrole

**Seu controle também pode ser uma ferramenta de produtividade.**

O **Crontrole** é um aplicativo desktop para Windows que transforma controles compatíveis com **XInput** em um painel de atalhos, comandos de teclado e ações de mouse. Use os botões do controle para executar tarefas do dia a dia e os analógicos para movimentar o cursor e rolar páginas.

**Windows 11 · C# · .NET 10 · WPF · XInput 1.4**

> **Alpha v1.0** é uma versão experimental. A janela e o executável já se chamam *Crontrole*; o projeto C# e a pasta local do perfil conservam o identificador `Kp4040.App` para compatibilidade.

## O que já funciona

- 🎮 **Identificar botão:** pressione um botão físico para localizar sua configuração.
- ⌨️ **Atalhos personalizáveis:** atribua teclas, combinações e comandos de mídia.
- 🖱️ **Mouse pelo controle:** analógico esquerdo move o cursor; direito controla o scroll vertical e horizontal.
- 🖱️ **Ações do mouse:** clique esquerdo, direito, duplo clique, voltar, avançar e rolagem.
- 🖱️ **Arrastar:** mantenha um botão mapeado para clique esquerdo pressionado e mova o stick esquerdo.
- ⌨️ **Combinar botões:** segure um botão mapeado para Alt e pressione outro mapeado para Tab.
- 💾 **Perfil local:** as configurações são salvas em JSON no Windows.
- ⚙️ **Controles de ativação:** ações e analógicos iniciam habilitados; as duas opções podem ser desmarcadas na janela.

O Crontrole **não** exige uma chave de API. O comando “Abrir ChatGPT” apenas abre o site no navegador; a integração direta com a API da OpenAI é uma possibilidade futura.

## Comece por aqui

Para executar **a partir do código-fonte**, você precisa do [SDK .NET 10 para Windows](https://dotnet.microsoft.com/download/dotnet/10.0) e de um controle reconhecido pelo Windows via XInput.

```powershell
git clone https://github.com/SrMahal/crontrole-win.git
cd crontrole-win
dotnet run --project .\src\Kp4040.App\Kp4040.App.csproj
```

O caminho do projeto ainda utiliza o nome técnico legado `Kp4040.App`, mantido temporariamente por compatibilidade.

**Prefere executar sem compilar?** Baixe o [pacote Windows x64](dist/Crontrole-Alpha-v1.0-win-x64.zip), extraia os quatro arquivos na mesma pasta e abra `Crontrole.exe`. Este pacote requer o **.NET 10 Desktop Runtime** instalado no Windows. O [SDK .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) também atende esse requisito.

## Documentação

| Quero… | Acessar |
| --- | --- |
| Instalar e executar no Windows | [Instalação](docs/instalacao.md) |
| Configurar botões, atalhos e analógicos | [Guia de uso](docs/uso.md) |
| Entender os módulos e o fluxo de dados | [Arquitetura](docs/arquitetura.md) |
| Modificar, compilar e testar o projeto | [Desenvolvimento](docs/desenvolvimento.md) |
| Conhecer limitações e cuidados | [Segurança e privacidade](docs/seguranca.md) |
| Acompanhar as próximas etapas | [Roadmap](docs/roadmap.md) |
| Preparar versões para distribuição | [Publicação de releases](docs/publicacao.md) |

**[Abrir o índice completo da documentação →](docs/README.md)**

## Estado do projeto

O Crontrole está em desenvolvimento experimental. A arquitetura atual funciona com um controle XInput por vez (primeiro índice disponível). Os comandos são executados pelo aplicativo no Windows, sem alterar o driver físico ou bloquear os sinais que jogos recebem.

**Criado por [SrMahal](https://github.com/SrMahal)** · [Repositório](https://github.com/SrMahal/crontrole-win) · [Issues e sugestões](https://github.com/SrMahal/crontrole-win/issues)

> Projeto independente, sem vínculo oficial com fabricantes de controles, Microsoft ou OpenAI. O repositório público ainda não contém uma licença de uso/distribuição; consulte o autor antes de redistribuir ou reutilizar o código fora dos direitos legais aplicáveis.
