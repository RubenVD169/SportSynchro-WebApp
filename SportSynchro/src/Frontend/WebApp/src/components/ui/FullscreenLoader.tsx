export function FullscreenLoader({ text }: { text?: string }) {
  return (
    <div className="fixed inset-0 flex items-center justify-center bg-slate-950 text-white">
      <div className="text-center">
        <div className="animate-spin h-8 w-8 border-2 border-white border-t-transparent rounded-full mx-auto mb-4" />
        <p className="text-sm text-slate-300">
          {text ?? "Loading…"}
        </p>
      </div>
    </div>
  );
}
