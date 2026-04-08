USE zoomcars_inventory;

CREATE TABLE IF NOT EXISTS ContactMessages (
    MessageId   INT AUTO_INCREMENT PRIMARY KEY,
    Name        VARCHAR(100) NOT NULL,
    Email       VARCHAR(100) NOT NULL,
    Phone       VARCHAR(20),
    Subject     VARCHAR(50),
    Message     TEXT NOT NULL,
    SubmittedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
