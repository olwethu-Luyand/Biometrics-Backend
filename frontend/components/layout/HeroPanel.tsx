import primeOakBg from "../../assets/prime_oak.jpeg";
import { motion } from "framer-motion";

export default function HeroPanel() {
  return (
    <div
      className="relative flex flex-col items-center justify-center bg-cover bg-center overflow-hidden"
      style={{ backgroundImage: `url(${primeOakBg})` }}
    >
      <div className="absolute inset-0 bg-black/30" />

      <motion.div
        animate={{ opacity: [0.4, 0.8, 0.4], scale: [1, 1.2, 1] }}
        transition={{ duration: 4, repeat: Infinity, ease: "easeInOut" }}
        className="absolute top-1/4 left-1/4 h-64 w-64 rounded-full bg-blue-500/20 blur-3xl"
      />

      <motion.div
        animate={{ opacity: [0.3, 0.7, 0.3], scale: [1, 1.3, 1] }}
        transition={{ duration: 5, repeat: Infinity, ease: "easeInOut", delay: 1 }}
        className="absolute bottom-1/3 right-1/4 h-80 w-80 rounded-full bg-cyan-500/15 blur-3xl"
      />

      <motion.div
        animate={{ opacity: [0.2, 0.6, 0.2], scale: [1, 1.5, 1] }}
        transition={{ duration: 6, repeat: Infinity, ease: "easeInOut", delay: 2 }}
        className="absolute top-1/2 right-1/3 h-48 w-48 rounded-full bg-blue-400/10 blur-3xl"
      />

      <motion.div
        initial={{ x: "-100%" }}
        animate={{ x: "100%" }}
        transition={{ duration: 3, repeat: Infinity, ease: "linear", delay: 1 }}
        className="absolute top-1/4 h-px w-1/2 bg-gradient-to-r from-transparent via-blue-400/80 to-transparent"
      />

      <motion.div
        initial={{ x: "-100%" }}
        animate={{ x: "100%" }}
        transition={{ duration: 4, repeat: Infinity, ease: "linear", delay: 3 }}
        className="absolute bottom-1/3 h-px w-2/3 bg-gradient-to-r from-transparent via-cyan-400/60 to-transparent"
      />

      <motion.div
        animate={{ opacity: [0, 0.8, 0] }}
        transition={{ duration: 2, repeat: Infinity, ease: "easeInOut", delay: 0.5 }}
        className="absolute top-1/3 left-1/2 h-32 w-32 rounded-full bg-blue-400/20 blur-2xl"
      />

      <motion.div
        animate={{ opacity: [0, 0.6, 0] }}
        transition={{ duration: 2.5, repeat: Infinity, ease: "easeInOut", delay: 1.5 }}
        className="absolute bottom-1/4 left-1/3 h-24 w-24 rounded-full bg-cyan-400/20 blur-2xl"
      />
    </div>
  );
}
