# Documentação da API Moto Rental

A API Moto Rental permite a gestão de motocicletas, entregadores, pedidos e notificações dentro de uma plataforma de locação de motos. Este guia fornece uma visão geral dos endpoints disponíveis e suas funcionalidades principais.

## Autenticação

- **Login (`POST /api/authentication/login`):** Autentica um usuário com base nas credenciais fornecidas. Se for o primeiro acesso e não existirem usuários no sistema, cria um usuário admin com as credenciais fornecidas.
  - **Permissões:** Público

- **Adicionar Usuário (`POST /api/authentication`):** Adiciona um novo usuário ao sistema. Requer que a senha atenda a critérios específicos.
  - **Permissões:** Admin

- **Listar Usuários (`GET /api/authentication`):** Lista todos os usuários do sistema, com opção de filtro por identificador de usuário ou nome de usuário.
  - **Permissões:** Admin

- **Atualizar Usuário (`PUT /api/authentication/{userid}`):** Atualiza informações de um usuário específico, incluindo a troca de funções.
  - **Permissões:** Admin

- **Desativar Usuário (`DELETE /api/authentication/{userid}`):** Desativa um usuário, marcando-o como inativo.
  - **Permissões:** Admin

## Entregadores

- **Adicionar Entregador (`POST /api/deliverypersons`):** Registra um novo entregador no sistema.
  - **Permissões:** Entregador (próprio registro)

- **Atualizar CNH (`PUT /api/deliverypersons/{id}/cnh`):** Atualiza o arquivo da CNH de um entregador específico.
  - **Permissões:** Entregador (próprio perfil)

- **Listar Entregadores (`GET /api/deliverypersons`):** Lista todos os entregadores registrados no sistema.
  - **Permissões:** Admin

## Motocicletas

- **Adicionar Moto (`POST /api/motos`):** Adiciona uma nova motocicleta ao sistema.
  - **Permissões:** Admin

- **Listar Motos (`GET /api/motos`):** Recupera uma lista de todas as motocicletas registradas no sistema.
  - **Permissões:** Admin

- **Atualizar Moto (`PUT /api/motos/{id}`):** Atualiza os dados de uma motocicleta existente no sistema.
  - **Permissões:** Admin

- **Desativar Moto (`DELETE /api/motos/{id}`):** Desativa uma motocicleta no sistema.
  - **Permissões:** Admin

## Pedidos

- **Criar Pedido (`POST /api/orders`):** Cria um novo pedido no sistema e notifica os entregadores elegíveis.
  - **Permissões:** Admin

- **Aceitar Pedido (`PUT /api/orders/accept/{orderId}/{deliveryPersonId}`):** Permite que um entregador aceite um pedido.
  - **Permissões:** Entregador (notificado para o pedido)

- **Completar Pedido (`PUT /api/orders/complete/{orderId}/{deliveryPersonId}`):** Marca um pedido como entregue.
  - **Permissões:** Entregador (que aceitou o pedido)

## Notificações

- **Listar Notificações (`GET /api/notifications`):** Recupera uma lista de todas as notificações do sistema.
  - **Permissões:** Admin


Considerando que você está procurando uma extensão da documentação começada anteriormente, vamos seguir com mais detalhes e aprofundamentos para os endpoints, focando em regras adicionais, erros comuns e dicas de uso. Observe que o conteúdo a seguir é uma continuação do README.md anteriormente proposto e deve ser anexado ao mesmo.

**README.md (Continuação):**

### Dicas Gerais de Uso

- **Autenticação e Autorização:** 
  - Todos os endpoints requerem autenticação via token JWT, exceto o login.
  - As permissões são definidas com base no papel (role) do usuário. Certifique-se de utilizar o token correto para acessar os recursos permitidos.

- **Tratamento de Erros:**
  - Em caso de erro, a API retorna um código de status HTTP apropriado, juntamente com uma mensagem de erro no corpo da resposta. É importante tratar esses erros adequadamente no lado do cliente.

### Erros Comuns

- **401 Unauthorized:** 
  - Ocorre quando um token de acesso não é fornecido, é inválido ou expirou. Certifique-se de estar logado e utilizar o token JWT no cabeçalho de autorização.

- **403 Forbidden:** 
  - Indica que o usuário autenticado não tem permissão para acessar o recurso solicitado. Verifique se o seu papel (role) permite o acesso ao endpoint.

- **404 Not Found:** 
  - O recurso solicitado não foi encontrado. Pode ocorrer ao tentar acessar uma entidade com um identificador que não existe.

- **500 Internal Server Error:** 
  - Erro genérico indicando um problema no servidor. Se persistir, entre em contato com o suporte técnico.

### Exemplos de Chamadas

- **Login:**
  ```json
  POST /api/authentication/login
  Body:
  {
      "username": "admin",
      "password": "Admin@123"
  }
  ```

- **Adicionar Moto:**
  ```json
  POST /api/motos
  Headers: Authorization: Bearer <Your_JWT_Token>
  Body:
  {
      "year": 2021,
      "model": "Yamaha YZF-R3",
      "licensePlate": "XYZ1234"
  }
  ```

### Considerações Finais

- **Segurança:** Mantenha suas credenciais seguras e não compartilhe seu token JWT.
- **Manutenção de Dados:** Regularmente, verifique e atualize as informações dos entregadores e motocicletas para garantir a precisão dos dados.
- **Feedback:** A API está em constante evolução. Seu feedback é valioso para melhorarmos continuamente.

Para mais informações, consulte a documentação detalhada do Swagger disponível em: `https://localhost:61812/swagger`

**Fim da Documentação**
