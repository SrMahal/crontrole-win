# Segurança e privacidade

[← Índice da documentação](README.md)

## Execução consciente

O Crontrole inicia com **execução de ações desativada**. A opção para mouse analógico também precisa da habilitação geral para atuar. Atalhos simulados podem alterar textos, executar comandos de interface ou clicar onde o cursor estiver; teste primeiro em aplicações sem dados sensíveis.

A captura de um botão e a gravação de um atalho suspendem a execução de ações durante esse processo. Na implementação atual, o aplicativo também procura evitar atalhos de teclado enviados para sua própria janela.

## Não é um driver

O programa lê **XInput 1.4** e utiliza a função `SendInput` da API Win32. Ele não instala driver em kernel, não modifica o dispositivo físico e não bloqueia a entrada original que jogos podem receber. O Windows pode restringir eventos enviados a aplicações de maior nível de privilégio.

## Dados locais

O perfil JSON é gravado em `%LOCALAPPDATA%\KP4040AIController\profiles\default.json`. Ele contém associações de botões e atalhos e deve ser tratado como preferência local. **Não adicione senhas, tokens nem chaves de API nesse arquivo.**

O aplicativo atual não usa API Key. A ação “Abrir ChatGPT” abre o endereço público do ChatGPT no navegador. Uma integração via API exigirá, em etapa futura, tratamento explícito de credenciais, permissões, custo e coleta de dados.

## Publicação de código e binários

- Antes de commits, revise arquivos `.env`, logs, perfis pessoais e outros arquivos sensíveis.
- As pastas `bin/` e `obj/` devem ficar fora do controle de versão; são produtos do build.
- Antes de distribuir executáveis, gere um pacote Release, teste no Windows e documente os requisitos e limitações.
- O repositório está público, mas **não possui licença identificada** neste momento. Visibilidade pública não equivale automaticamente a uma licença de reutilização.

## Projeto independente

Crontrole não é afiliado oficialmente à Microsoft, fabricantes de controles ou OpenAI.
