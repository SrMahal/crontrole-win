# Guia de uso e mapeamento

[← Índice da documentação](README.md)

## Ativar com segurança

O Crontrole monitora o dispositivo sem disparar comandos até você marcar **Habilitar execução de ações**. Para controlar o cursor e a rolagem, marque também **Analógicos: mouse e scroll**. O uso do segundo checkbox depende do primeiro.

## Identificar um botão

1. Clique em **Identificar botão**.
2. Pressione **um botão físico** do controle; não mova os sticks durante a captura.
3. A lista selecionará a entrada XInput correspondente.
4. Escolha uma ação na linha destacada.
5. Para gravar um atalho personalizado, selecione a ação apropriada e clique em **Gravar atalho**; pressione a combinação no teclado.
6. Habilite as ações e teste fora da janela de configuração.

No modo de gravação, **Esc** sozinho cancela. Para mapear Esc, utilize o preset correspondente. Certos atalhos globais reservados pelo Windows não podem ser capturados ou reproduzidos de modo confiável.

## Exemplos de ações disponíveis

| Grupo | Exemplos |
| --- | --- |
| Teclado | Enter, Tab, Esc, Espaço, Backspace, Delete, setas, Home, End, Page Up/Down |
| Atalhos | Ctrl+C, Ctrl+V, Ctrl+A, Ctrl+Z; combinações personalizadas com modificadores |
| Mouse | Clique esquerdo/direito/meio, duplo clique, botões laterais, rolagem nas quatro direções |
| Mídia | Play/Pause, avançar/voltar faixa, volume e mudo |
| Navegador | Abrir ChatGPT (abre `https://chatgpt.com/`) |

O projeto **não** cria uma nova API do ChatGPT e **não** envia prompts diretamente à OpenAI nesta etapa.

## Analógicos

- **Esquerdo:** LX/LY movem o cursor do Windows.
- **Direito:** RY gera rolagem vertical e RX, horizontal.
- A rotina de mouse usa uma thread independente da janela WPF, com intervalo **alvo** de cerca de 8 ms, sujeito ao agendamento real do Windows.
- A zona morta implementada no serviço atual é **20%**. O ajuste pela interface ainda não está disponível.

## Gatilhos e botões

Os botões digitais XInput possuem eventos de pressionamento e soltura. LT e RT entram no mapeamento como botões virtuais quando excedem o limiar `TriggerThreshold`, que inicia em `180/255` no perfil padrão. As ações de botão são disparadas por uma nova pressão, não continuamente a cada atualização.

## Perfil local

As preferências e atalhos gravados ficam no Windows em:

```text
%LOCALAPPDATA%\KP4040AIController\profiles\default.json
```

O nome dessa pasta é um **identificador interno legado**, mantido na base atual para não perder configurações existentes. Faça cópia de segurança antes de instalar versões experimentais.

Estrutura simplificada de exemplo:

```json
{
  "SchemaVersion": 1,
  "Name": "Padrao",
  "TriggerThreshold": 180,
  "Mappings": {
    "A": "Copy",
    "B": "Paste",
    "X": "Enter",
    "Y": "OpenChatGpt"
  },
  "Shortcuts": {}
}
```

O arquivo contém outras entradas de mapeamento. **Não coloque chaves de API, tokens ou senhas nesse JSON.**

## Limitações atuais

- Reconhece o **primeiro** controle XInput encontrado entre os índices 0 a 3.
- A identificação localiza um botão digital/gatilho por vez; movimentos dos sticks não são capturados como botões.
- Não altera drivers, não bloqueia o gamepad para jogos e não grava macros longas.
- Atalhos sintéticos podem ser bloqueados por janelas privilegiadas do Windows.

[Leia também: Segurança e privacidade](seguranca.md).
