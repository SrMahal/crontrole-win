# Publicação de releases

[← Índice da documentação](README.md)

## Identidade e versão

- **Produto:** Crontrole.
- **Primeiro lançamento pretendido:** Alpha v1.0.
- **Tag recomendada:** `v1.0.0-alpha.1` (criar quando a versão estiver pronta).
- **Tipo:** *Pre-release*, enquanto o aplicativo estiver experimental.
- **Branch de origem:** `main` após revisão dos arquivos.

A tag de Release **não renomeia** o assembly interno nem atualiza o título da janela do aplicativo. Conclua a migração visual/técnica antes de afirmar que um binário já se chama Crontrole.

## Criar pacote Windows x64

No Windows, com o .NET 10 SDK instalado e a compilação validada:

```powershell
dotnet publish .\src\Kp4040.App\Kp4040.App.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -o .\dist\win-x64

Compress-Archive -Path .\dist\win-x64\* `
  -DestinationPath .\Crontrole-Alpha-v1.0-win-x64.zip -Force
```

Esse nome é o **nome de distribuição do ZIP**. O executável interno ainda será `Kp4040.App.exe` enquanto `AssemblyName` e o projeto não forem renomeados.

## Checklist antes de publicar

1. Executar `dotnet build` e testar a janela no Windows 11.
2. Validar botões, identificação, teclas, mouse, scroll e desligamento de ações.
3. Verificar se o perfil JSON antigo continua carregando.
4. Excluir `bin/`/`obj/` do versionamento, conferir `.gitignore` e revisar segredos.
5. Atualizar o README e notas de lançamento conforme as funcionalidades realmente testadas.
6. Anexar o ZIP do executável **somente após testá-lo**.
7. Criar a tag apontando para o commit correto e marcar **Pre-release**.

A página para publicar é [Releases → New release](https://github.com/SrMahal/crontrole-win/releases/new). A publicação é uma ação separada; preparar a documentação não publica uma versão automaticamente.
