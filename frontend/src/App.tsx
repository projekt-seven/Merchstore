import { BrowserRouter, Routes, Route } from "react-router-dom";
import CreateProductForm from "./admin/CreateProductForm";
import ProductDetails from "./admin/ProductDetails";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/admin/create" element={<CreateProductForm />} />
        <Route path="/admin/product/:id" element={<ProductDetails />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
