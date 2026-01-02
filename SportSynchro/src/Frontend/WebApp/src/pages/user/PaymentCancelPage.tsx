export default function PaymentCancelPage() {
  return (
    <div className="max-w-md mx-auto mt-20 text-center">
      <h1 className="text-2xl font-bold text-white mb-4">
        Payment cancelled
      </h1>

      <p className="text-gray-400 mb-6">
        The payment was cancelled. No charges were made.
      </p>

      <a
        href="/"
        className="inline-block bg-gray-600 hover:bg-gray-700 text-white px-4 py-2 rounded font-medium"
      >
        Back to dashboard
      </a>
    </div>
  );
}
