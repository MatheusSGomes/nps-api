### Visão Geral do NPS

O NPS (Net Promoter Score) é uma métrica de satisfação que mede a probabilidade de os clientes recomendarem uma empresa para amigos ou familiares. Os clientes respondem à pergunta: *"De 0 a 10, o quanto você recomendaria nossa empresa?"* As respostas são classificadas em três grupos:

- **Promotores (9-10)**: Clientes altamente satisfeitos, que provavelmente recomendariam a empresa.
- **Neutros (7-8)**: Clientes satisfeitos, mas menos propensos a recomendar.
- **Detratores (0-6)**: Clientes insatisfeitos, que podem compartilhar experiências negativas.

O cálculo do NPS é feito subtraindo a porcentagem de Detratores da porcentagem de Promotores:
\[
\text{NPS} = \% \text{ Promotores} - \% \text{ Detratores}
\]

### Requisitos da API para o Formulário de NPS

#### Requisitos Funcionais

1. **Cadastro de Respostas de NPS**:
   
   - Endpoint para registrar a resposta do cliente com uma nota (0 a 10).
   - Receber opcionalmente dados complementares, como nome do cliente e comentário.

2. **Consulta de NPS Geral**:
   
   - Endpoint para calcular e retornar o NPS atual com base em todas as respostas registradas.

3. **Consulta de Respostas Detalhadas**:
   
   - Endpoint para listar todas as respostas, com filtros opcionais (por nota ou período de tempo).

4. **Classificação Automática**:
   
   - A API deve categorizar automaticamente cada resposta em Promotor, Neutro ou Detrator.

5. **Relatório Resumido**:
   
   - Endpoint para fornecer um resumo com contagem de Promotores, Neutros e Detratores e o NPS atual.

#### Requisitos Não Funcionais

1. **Segurança**:
   
   - Autenticação e autorização básica para proteger os endpoints de consulta e cálculo do NPS.

2. **Escalabilidade**:
   
   - Estrutura modular para suportar aumento no número de respostas.

3. **Manutenibilidade**:
   
   - Arquitetura limpa para fácil manutenção, aplicando padrões como *Repository Pattern* para acesso aos dados.

4. **Confiabilidade**:
   
   - Testes unitários cobrindo as funcionalidades principais, como o cálculo do NPS e classificação automática, para garantir precisão e estabilidade.

### Tarefas e Subtarefas para Implementação

#### Tarefa 1: Configuração do Projeto e Estrutura Base

- **Subtarefas**:
  - Configurar o projeto em .NET 8.
  - Criar estrutura inicial com pastas para *Controllers*, *Services*, *Repositories*, *Models*, e *DTOs*.
  - Configurar dependências para TDD (ex.: xUnit, Moq para testes).

#### Tarefa 2: Implementar Cadastro de Respostas de NPS

- **Subtarefas**:
  - Criar o modelo `NpsResponse` com propriedades: `Score`, `CustomerName`, `Comment`, `Category`.
  - Implementar *DTO* para receber dados de entrada.
  - Criar controlador e endpoint `POST /nps/responses` para registro de resposta.
  - Implementar serviço para categorizar resposta em Promotor, Neutro ou Detrator.
  - Aplicar testes unitários para validação de notas e categorização.

#### Tarefa 3: Implementar Cálculo de NPS

- **Subtarefas**:
  - Criar endpoint `GET /nps/score` para calcular o NPS.
  - Implementar lógica no serviço para calcular o NPS com base nas respostas registradas.
  - Adicionar testes unitários para garantir que o cálculo do NPS esteja correto.

#### Tarefa 4: Consulta de Respostas Detalhadas

- **Subtarefas**:
  - Criar endpoint `GET /nps/responses` com parâmetros de filtro (ex.: por data, por categoria).
  - Implementar o repositório para listar as respostas.
  - Validar com testes unitários o funcionamento dos filtros.

#### Tarefa 5: Implementar Relatório Resumido

- **Subtarefas**:
  - Criar endpoint `GET /nps/summary` para retornar contagem de Promotores, Neutros e Detratores, e o NPS atual.
  - Implementar o serviço para obter as contagens e calcular o NPS.
  - Adicionar testes para garantir precisão nos cálculos e contagens.

#### Tarefa 6: Autenticação e Autorização

- **Subtarefas**:
  - Implementar autenticação básica para endpoints de consulta e relatório.
  - Adicionar verificação de permissões nos controladores.
  - Implementar testes para garantir que apenas usuários autenticados possam acessar os dados.

#### Tarefa 7: Documentação e Testes Finais

- **Subtarefas**:
  - Documentar endpoints com Swagger.
  - Realizar testes integrados para verificar o funcionamento dos endpoints.
  - Preparar uma breve documentação para o uso da API e instruções de instalação/configuração.

Essas etapas cobrem desde a configuração do projeto até o teste final da API, com foco em uma abordagem modular e baseada em testes para garantir a qualidade do código.
