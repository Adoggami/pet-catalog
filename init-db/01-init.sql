-- Initialization script for PetCatalog database
-- This script will be executed automatically when the PostgreSQL container starts

-- Create the pets table if it doesn't exist
CREATE TABLE IF NOT EXISTS pets (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    species VARCHAR(50) NOT NULL,
    breed VARCHAR(100),
    age INTEGER,
    color VARCHAR(50),
    weight DECIMAL(5,2),
    description TEXT,
    image_url VARCHAR(255),
    is_available BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_pets_available ON pets (is_available);
CREATE INDEX IF NOT EXISTS idx_pets_species ON pets (species);

-- Insert some sample data for development
INSERT INTO pets (name, species, breed, age, color, weight, description, is_available) VALUES
('Buddy', 'Dog', 'Golden Retriever', 3, 'Golden', 25.5, 'Friendly and energetic dog', true),
('Whiskers', 'Cat', 'Persian', 2, 'White', 4.2, 'Calm and affectionate cat', true),
('Max', 'Dog', 'German Shepherd', 5, 'Brown', 30.0, 'Loyal and intelligent dog', true),
('Luna', 'Cat', 'Siamese', 1, 'Cream', 3.8, 'Playful kitten', true),
('Charlie', 'Dog', 'Labrador', 4, 'Black', 28.3, 'Great family dog', false)
ON CONFLICT (id) DO NOTHING;

-- Update the sequence to avoid conflicts
SELECT setval('pets_id_seq', (SELECT MAX(id) FROM pets));

GRANT ALL PRIVILEGES ON DATABASE petcatalog TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO postgres;
