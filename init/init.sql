CREATE TABLE usuario (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    percentual_corretagem DECIMAL(5,2) NOT NULL
);

CREATE TABLE ativo (
    id INT PRIMARY KEY AUTO_INCREMENT,
    codigo VARCHAR(10) NOT NULL UNIQUE,
    nome VARCHAR(100) NOT NULL
);

CREATE TABLE operacoes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_id INT NOT NULL,
    ativo_id INT NOT NULL,
    quantidade INT NOT NULL,
    preco_unitario DECIMAL(10,2) NOT NULL,
    tipo_operacao ENUM('COMPRA', 'VENDA') NOT NULL,
    corretagem DECIMAL(10,2) NOT NULL,
    data_hora DATETIME NOT NULL,
    FOREIGN KEY (usuario_id) REFERENCES usuario(id),
    FOREIGN KEY (ativo_id) REFERENCES ativo(id)
);

CREATE TABLE cotacao (
    id INT PRIMARY KEY AUTO_INCREMENT,
    ativo_id INT NOT NULL,
    preco_unitario DECIMAL(10,2) NOT NULL,
    data_hora DATETIME NOT NULL,
    FOREIGN KEY (ativo_id) REFERENCES ativo(id)
);

CREATE TABLE posicao (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_id INT NOT NULL,
    ativo_id INT NOT NULL,
    quantidade INT NOT NULL,
    preco_medio DECIMAL(10,2) NOT NULL,
    pnl DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (usuario_id) REFERENCES usuario(id),
    FOREIGN KEY (ativo_id) REFERENCES ativo(id)
);

-- Índices para performance
CREATE INDEX idx_oper_usuario_ativo_data ON operacoes (usuario_id, ativo_id, data_hora);
CREATE INDEX idx_cot_ativo_data ON cotacao (ativo_id, data_hora DESC);
CREATE INDEX idx_pos_usuario_ativo ON posicao (usuario_id, ativo_id);
