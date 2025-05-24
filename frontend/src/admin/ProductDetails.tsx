import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";

type Product = {
  id: string;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  category: string;
  imageUrl?: string;
  tags: string[];
};

export default function ProductDetails() {
  const { id } = useParams();
  const [product, setProduct] = useState<Product | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        const response = await axios.get(`/api/admin/products/${id}`);
        setProduct(response.data);
      } catch (err) {
        console.error(err);
        setError("Kunde inte hämta produkten");
      }
    };

    fetchProduct();
  }, [id]);

  if (error) return <p>{error}</p>;
  if (!product) return <p>Laddar produkt...</p>;

  return (
    <div>
      <h2>✅ Produkten har skapats!</h2>
      <p>
        <strong>Namn:</strong> {product.name}
      </p>
      <p>
        <strong>Beskrivning:</strong> {product.description}
      </p>
      <p>
        <strong>Pris:</strong> {product.price} SEK
      </p>
      <p>
        <strong>Lager:</strong> {product.stockQuantity}
      </p>
      <p>
        <strong>Kategori:</strong> {product.category}
      </p>
      {product.imageUrl && (
        <img src={product.imageUrl} alt={product.name} width="300" />
      )}

      <div>
        <strong>Taggar:</strong> {product.tags?.join(", ") ?? "Inga taggar"}
      </div>
    </div>
  );
}
