import { useForm } from "react-hook-form";
import axios from "axios";
import "./CreateProductForm.scss";
import { useNavigate } from "react-router-dom";

type FormData = {
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  category: string;
  imageUrl: string;
  tags: string;
};

export default function CreateProductForm() {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting, isSubmitSuccessful },
  } = useForm<FormData>();

  const onSubmit = async (data: FormData) => {
    const payload = {
      ...data,
      tags: data.tags.split(",").map((tag) => tag.trim()),
    };

    try {
      const response = await axios.post("/api/admin/products", payload);
      navigate(`/admin/product/${response.data.id}`);
      reset();
    } catch (error) {
      console.error("Kunde inte skapa produkt:", error);
      alert("Något gick fel vid skapandet av produkten.");
    }
  };

  return (
    <form className="create-product-form" onSubmit={handleSubmit(onSubmit)}>
      <h2>Skapa ny produkt</h2>

      <label>
        Namn:
        <input {...register("name", { required: true })} />
        {errors.name && <span>Namnet är obligatoriskt</span>}
      </label>

      <label>
        Beskrivning:
        <textarea {...register("description", { required: true })} />
        {errors.description && <span>Beskrivning krävs</span>}
      </label>

      <label>
        Pris (SEK):
        <input
          type="number"
          step="0.01"
          {...register("price", { required: true })}
        />
      </label>

      <label>
        Lagerantal:
        <input
          type="number"
          {...register("stockQuantity", { required: true })}
        />
      </label>

      <label>
        Kategori:
        <input {...register("category", { required: true })} />
      </label>

      <label>
        Bild-URL:
        <input {...register("imageUrl")} />
      </label>

      <label>
        Taggar (komma-separerat):
        <input {...register("tags")} />
      </label>

      <button type="submit" disabled={isSubmitting}>
        Skapa produkt
      </button>

      {isSubmitSuccessful && <p className="success">Produkten har skapats!</p>}
    </form>
  );
}
