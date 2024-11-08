# Sistema de Gestão de Eventos e Inscrições Online

## Descrição

O **Sistema de Gestão de Eventos e Inscrições Online** é uma plataforma robusta e intuitiva desenvolvida para facilitar o gerenciamento de eventos como workshops, palestras e conferências. O sistema permite que administradores configurem eventos, definindo datas, locais e capacidade de público, enquanto os usuários podem visualizar a lista de eventos, se inscrever e receber confirmações automáticas por e-mail.

## Funcionalidades

- **Autenticação e Autorização**
  - Sistema de login com perfis de administrador e participante usando **ASP.NET Identity**.
  - Autenticação via OAuth2 (login com Google ou Facebook) para facilitar o cadastro de usuários.

- **Cadastro de Eventos**
  - Administradores podem criar, editar e excluir eventos, com opções para definir:
    - Título do evento
    - Descrição
    - Data e horário
    - Local
    - Capacidade máxima

- **Inscrição em Eventos**
  - Usuários autenticados podem se inscrever até o limite de vagas.
  - Sistema envia e-mails de confirmação de inscrição com **SendGrid** ou SMTP.

- **Notificações**
  - Envio de lembretes automáticos aos participantes antes do evento.

- **Relatórios Gerenciais**
  - Administradores podem acessar relatórios, incluindo:
    - Número total de participantes
    - Taxa de ocupação dos eventos
    - Participantes por evento

- **Dashboard Administrativo**
  - Painel de controle com visualizações gráficas de estatísticas usando **Chart.js**.

- **Exportação de Dados**
  - Exportação de dados dos inscritos para PDF ou Excel, com **iTextSharp** e **EPPlus** para geração de relatórios.

- **Segurança**
  - Proteção com autenticação e autorização, garantindo o acesso seguro para administradores e usuários.

## Stack Tecnológica

- **Frontend**: React
- **Backend**: ASP.NET Core 8.0
- **Banco de Dados**: SQL Server
- **Autenticação**: ASP.NET Identity e OAuth2
- **Relatórios e Exportação**: iTextSharp, EPPlus
- **E-mail**: SendGrid para envio de confirmações e notificações
- **Visualizações Gráficas**: Chart.js

## Público-Alvo

Ideal para pequenas e grandes empresas que precisam organizar eventos frequentemente, o sistema é uma solução prática e escalável para o gerenciamento de eventos e participantes, facilitando a organização e a comunicação com os inscritos.
