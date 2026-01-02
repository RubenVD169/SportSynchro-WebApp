export default function PaymentSuccessPage() {
  return (
    <div className="max-w-md mx-auto mt-20 text-center">
      <h1 className="text-2xl font-bold text-white mb-4">
        Payment successful
      </h1>

      <p className="text-gray-400 mb-6">
        Your payment was received. Premium access is being activated.
      </p>

      <a
        href="/"
        className="inline-block bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded font-medium"
      >
        Back to dashboard
      </a>
    </div>
  );
}
