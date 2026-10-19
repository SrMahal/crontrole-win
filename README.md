# 🎮 Crontrole

**Seu controle também pode ser uma ferramenta de produtividade.**

O **Crontrole** é um aplicativo desktop para Windows que transforma controles compatíveis com **XInput** em um painel de atalhos, comandos de teclado e ações de mouse. Use os botões do controle para executar tarefas do dia a dia e os analógicos para movimentar o cursor e rolar páginas.

**Windows 11 · C# · .NET 10 · WPF · XInput 1.4**

> **Alpha v1.0** é a identidade do primeiro lançamento experimental planejado. A base de código atualmente publicada no repositório corresponde à **Alpha 0.5.1**; os nomes internos e o título da janela ainda serão migrados para *Crontrole*. Consulte o [roteiro de versões](docs/roadmap.md).

## O que já funciona

- 🎮 **Identificar botão:** pressione um botão físico para localizar sua configuração.
- ⌨️ **Atalhos personalizáveis:** atribua teclas, combinações e comandos de mídia.
- 🖱️ **Mouse pelo controle:** analógico esquerdo move o cursor; direito controla o scroll vertical e horizontal.
- 🖱️ **Ações do mouse:** clique esquerdo, direito, duplo clique, voltar, avançar e rolagem.
- 💾 **Perfil local:** as configurações são salvas em JSON no Windows.
- 🔒 **Ativação consciente:** as ações começam desabilitadas a cada abertura.

O Crontrole **não** exige uma chave de API. O comando “Abrir ChatGPT” apenas abre o site no navegador; a integração direta com a API da OpenAI é uma possibilidade futura.

## Comece por aqui

Para executar **a partir do código-fonte**, você precisa do [SDK .NET 10 para Windows](https://dotnet.microsoft.com/download/dotnet/10.0) e de um controle reconhecido pelo Windows via XInput.

```powershell
git clone https://github.com/SrMahal/crontrole-win.git
cd crontrole-win
dotnet run --project .\src\Kp4040.App\Kp4040.App.csproj
```

O caminho do projeto ainda utiliza o nome técnico legado `Kp4040.App`, mantido temporariamente por compatibilidade.

**Prefere baixar uma versão para teste?** Confira a área de [Releases](https://github.com/SrMahal/crontrole-win/releases). A disponibilidade de executáveis depende dos arquivos anexados a cada lançamento.

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
