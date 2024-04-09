# Aluguel de motos

- [Wiki](https://github.com/BrunoICardoso/MotoRental/wiki)


## Descrição Breve
Um sistema de aluguel de motos que permite aos administradores gerenciar eficientemente e aos entregadores gerir suas locações.

## Começando

### Pré-requisitos
Antes de começar, certifique-se de ter o Docker e o Docker Compose instalados em sua máquina. Isso permitirá a execução do ambiente de desenvolvimento e produção em contêineres.

- [Instalar Docker](https://docs.docker.com/get-docker/)
- [Instalar Docker Compose](https://docs.docker.com/compose/install/)

### Instalação
Para configurar o projeto localmente, siga estes passos:

1. Clone o repositório do projeto:
   ```sh
   git clone [https://github.com/motorental/motorental.git](https://github.com/BrunoICardoso/MotoRental)
   ```

2. Navegue até a pasta do projeto e execute o Docker Compose:
   ```sh
   cd motorental
   docker-compose up -d
   ```

3. Aguarde o Docker baixar as imagens necessárias e criar os contêineres.

4. Após o processo, confirme se os serviços estão rodando:
   ```sh
   docker-compose ps
   ```

## Uso

Para utilizar o sistema, siga as instruções abaixo:

- Acesse a API através do endereço `http://localhost:8000/api`.
- Utilize as rotas disponíveis conforme documentação do Swagger acessível em `http://localhost:8000/swagger`.
- Para fazer login como administrador ou entregador, use os seguintes endpoints:
  - `/auth/login` para autenticação
  - Utilize o token JWT recebido nas demais requisições como Bearer Token.


Aqui está uma sugestão de texto para a seção de iniciação do banco de dados na Wiki do GitHub do projeto, em português:

---
### Criação Automática do Banco de Dados

Para simplificar o processo de desenvolvimento, o projeto está configurado para verificar e criar o banco de dados automaticamente, caso ele ainda não exista. Isso é realizado através de um script de inicialização que executa durante a fase de startup da aplicação.

O código responsável pela verificação e criação do banco de dados está localizado no método `EnsureDatabaseCreated` na classe `DatabaseManager`. Esse método faz parte do código de inicialização da aplicação e é invocado antes da aplicação começar a servir requisições.

Quando a aplicação é iniciada, o método `EnsureDatabaseCreated`:

1. Abre uma conexão com o PostgreSQL utilizando as credenciais fornecidas no arquivo de configuração.
2. Verifica a existência do banco de dados destinado ao projeto.
3. Caso o banco de dados não exista, o método executa um comando SQL para criar o banco.

### Migrations

Além da criação do banco de dados, o projeto utiliza o FluentMigrator para gerenciar as migrations, que são responsáveis pela criação das tabelas e pela manutenção da estrutura do banco de dados. As classes de migration estão localizadas no assembly do projeto e são aplicadas automaticamente durante o startup da aplicação através do serviço `IMigrationRunner`.

A execução das migrations garante que o banco de dados esteja sempre atualizado com a última versão do esquema necessário para a aplicação funcionar corretamente.
---

## Contato
Bruno – bruno.inacio88@gmail.com
