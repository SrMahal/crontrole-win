# TASK 005 — Identificação física e ações de entrada

- [x] Botão Identificar e seleção da linha XInput por pressão.
- [x] Ações configuráveis de mouse: botões, duplo clique e roda.
- [x] Gravação de combinação do teclado na UI, com persistência JSON v1 retrocompatível.
- [x] Gravação e identificação suspendem disparos do controle.
- [x] Serviços isolados do driver XInput e do movimento analógico.
- [ ] Validar compilação e testes reais no Windows 11.

## Checklist manual
1. Execute com execução de ações desligada. Clique Identificar botão e pressione B: deve focar e destacar B.
2. B = Mouse clique esquerdo; habilite ações e clique fora do controlador antes de testar.
3. A = Teclado · Gravar tecla/atalho; clique Gravar e pressione Ctrl+Shift+S. Reinicie e confira que o atalho persistiu.
4. Abra o Bloco de Notas e teste em janela diferente; desabilite ações e confirme que os botões não enviam teclas.
5. Teste L2/R2 com limiar 180 e os dois analógicos como no Alpha 0.4.
6. Faça backup de `%LOCALAPPDATA%\KP4040AIController\profiles\default.json` antes da primeira execução atualizada.
