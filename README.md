# Aluguel de motos

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

## Contato
Bruno – bruno.inacio88@gmail.com

```

Certifique-se de substituir `seuprojeto`, `YOUR_JWT_TOKEN`, e as informações de contato pelas informações reais do seu projeto. Além disso, adapte as URLs e os exemplos de comandos conforme a configuração e as rotas específicas do seu projeto.
